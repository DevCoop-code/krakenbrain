#if UNITY_VERSION < 450
    #include "iPhone_View.h"
#endif

#include <UIKit/UIKit.h>
#include <AVFoundation/AVFoundation.h>
#include <Metal/Metal.h>

#include <stdlib.h>
#include <string.h>
#include <stdint.h>


//@import StableDiffusionFramework;
#import <StableDiffusionFramework/StableDiffusionFramework-Swift.h>

@interface iOSInterface: NSObject
{
    id<MTLDevice> device;
    
    CVMetalTextureCacheRef textureCache;
    id<MTLTexture> texture;
}
- (void)loadWeight:(NSString*)weightURL;
- (void)generateTextToImage;
@end

ImageGenerator* imgGenerator;
//static NSString* prompt = @"a photo of an astronaut riding a horse on mars";
//static NSString* negativePrompt = @"lowres, bad anatomy, bad hands, text, error, missing fingers, extra digit, fewer digits, cropped, worst quality, low quality, normal quality, jpeg artifacts, blurry, multiple legs, malformation";

@implementation iOSInterface

typedef void ( *IntPointerCallbackListener ) (intptr_t);
static IntPointerCallbackListener g_intPointerCallbackListener = NULL;

- (void)loadWeight:(NSString*)weightURL
{
//    NSLog(@"[iOSInterface] LoadWeight : %@", weightURL);
//
//    NSString* weightPath = [[NSBundle mainBundle]pathForResource:weightURL ofType:NULL];
//    if (NULL == weightPath)
//        NSLog(@"Fatal Error: Failed to find the CoreML Models");
//
    NSLog(@"[iOSInterface] LoadPath : %@", weightURL);
    
    device = MTLCreateSystemDefaultDevice();
    
    imgGenerator = [[ImageGenerator alloc] init];
    [imgGenerator setSDModelWeightWithWeight:weightURL];
}

- (void)generateTextToImage:(NSString*)prompt nprompt:(NSString*)nprompt guidanceScale:(float)guidanceScale seed:(float)seed stepCount:(int)stepCount imageCount:(int)imageCount disableSafety:(bool)disableSafety
{
    NSLog(@"[iOSInterface] generateTextToImage");
    [imgGenerator generatedTextToImages:prompt :nprompt guidanceScale:guidanceScale :seed :stepCount :imageCount :disableSafety :nil :0];
    
    [imgGenerator addSetSDPipelineDoneFuncPointerWithClosure:^(NSInteger responseCode) {
        NSLog(@"[iOSInterface] ResponseCode : %ld", (long)responseCode);
    }];
    [imgGenerator addGeneratedImageResultFuncPointerWithClosure:^(UIImage* image) {
        NSLog(@"[iOSInterface] Callback Func Call");
        
        // https://liveupdate.tistory.com/445
        
        // UIImage to PixelBuffer
        CGImageRef imageRef = image.CGImage;
        
        NSDictionary* options = @{
            (NSString*)kCVPixelBufferCGImageCompatibilityKey: @YES,
            (NSString*)kCVPixelBufferCGBitmapContextCompatibilityKey: @YES,
            (NSString*)kCVPixelBufferMetalCompatibilityKey: @YES
        };
        CVPixelBufferRef pixelBuffer = NULL;
        CVReturn status = CVPixelBufferCreate(kCFAllocatorDefault,
                                              CGImageGetWidth(imageRef),
                                              CGImageGetHeight(imageRef),
                                              kCVPixelFormatType_32BGRA,
                                              (__bridge CFDictionaryRef)options,
                                              &pixelBuffer);
        if (status != kCVReturnSuccess)
            NSLog(@"Operation Failed");
            
        NSParameterAssert(status == kCVReturnSuccess && pixelBuffer != NULL);
        
        CVPixelBufferLockBaseAddress(pixelBuffer, 0);
        void *pxdata = CVPixelBufferGetBaseAddress(pixelBuffer);

        CGColorSpaceRef rgbColorSpace = CGColorSpaceCreateDeviceRGB();
        CGContextRef context = CGBitmapContextCreate(pxdata,
                                                     CGImageGetWidth(imageRef),
                                                     CGImageGetHeight(imageRef),
                                                     8,
                                                     /*4 * CGImageGetWidth(imageRef),*/
                                                     CVPixelBufferGetBytesPerRow(pixelBuffer),
                                                     rgbColorSpace,
                                                     kCGImageByteOrder32Little | kCGImageAlphaPremultipliedFirst);
        NSParameterAssert(context);

        CGContextConcatCTM(context, CGAffineTransformMakeRotation(0));
        CGAffineTransform flipVertical = CGAffineTransformMake(1, 0, 0, -1, 0, CGImageGetHeight(imageRef));
        CGContextConcatCTM(context, flipVertical);
        CGAffineTransform flipHorizontal = CGAffineTransformMake(-1, 0, 0, 1, CGImageGetWidth(imageRef), 0);
        CGContextConcatCTM(context, flipHorizontal);

        CGContextDrawImage(context, CGRectMake(0, 0, CGImageGetWidth(imageRef), CGImageGetHeight(imageRef)), imageRef);
        CGColorSpaceRelease(rgbColorSpace);
        CGContextRelease(context);

        CVPixelBufferUnlockBaseAddress(pixelBuffer, 0);
        

         //PixelBuffer to CVMetalTexture
        size_t width = CVPixelBufferGetWidth(pixelBuffer);
        size_t height = CVPixelBufferGetHeight(pixelBuffer);
        
        CVMetalTextureRef textureOut = nil;
        
        NSLog(@"PixelBuffer width: %zu, height: %zu", width, height);
        
        CVReturn result = CVMetalTextureCacheCreate(kCFAllocatorDefault,
                                                    nil,
                                                    self->device,
                                                    nil,
                                                    &(self->textureCache));
        if (result == kCVReturnSuccess)
        {
            // https://developer.apple.com/forums/thread/670457
            CVReturn ret = CVMetalTextureCacheCreateTextureFromImage(kCFAllocatorDefault,
                                                      self->textureCache,
                                                      pixelBuffer,
                                                      nil,
                                                      MTLPixelFormatBGRA8Unorm,
                                                      width,
                                                      height,
                                                      0,
                                                      &textureOut);
            
            NSLog(@"CVMetalTextureCacheCreate Success, %d", ret);
            NSParameterAssert(ret == kCVReturnSuccess && textureOut != NULL);
            
            self->texture = CVMetalTextureGetTexture(textureOut);
            NSLog(@"MetalTexture: %@", self->texture);
            g_intPointerCallbackListener((uintptr_t)(__bridge void*)self->texture);
        }
        else
        {
            NSLog(@"Failed to make texture Cache");
        }
        
        if (self->textureCache != nil)
        {
            CFRelease(self->textureCache);
            self->textureCache = nil;
        }
        if (textureOut != nil)
        {
            CVBufferRelease(textureOut);
            textureOut = nil;
        }
    }];
}

@end

/*
 Communication to Unity
 */
static iOSInterface* _GetInterface()
{
    static iOSInterface* _interfaceInstance = nil;
    if (!_interfaceInstance)
        _interfaceInstance = [[iOSInterface alloc] init];
    
    return _interfaceInstance;
}

static NSString* _GetString(const char* charText)
{
    NSString* textStr = nil;
    textStr = [NSString stringWithUTF8String: charText];
    return textStr;
}

extern "C" void LoadWeight(const char* weightURL)
{
    [_GetInterface() loadWeight:_GetString(weightURL)];
}

extern "C" void generateTextToImg(const char* prompt, const char* nprompt, float guidanceScale, float seed, int stepCount, int imageCount, bool disableSafety, IntPointerCallbackListener funcPtr)
{
    g_intPointerCallbackListener = funcPtr;
    
    [_GetInterface() generateTextToImage:_GetString(prompt)
                                 nprompt:_GetString(nprompt)
                           guidanceScale:guidanceScale
                                    seed:seed
                               stepCount:stepCount
                              imageCount:imageCount
                           disableSafety:disableSafety];
}
