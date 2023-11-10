#import "UnityAppController.h"
#import "AppDelegateListener.h"
#import <AVFoundation/AVAudioSession.h>

@interface HikerAppController : UnityAppController <AppDelegateListener>
{
    BOOL _pausedByInterruption;
}
@end

@implementation HikerAppController

- (instancetype)init
{
    self = [super init];
    if (self) {
        UnityRegisterAppDelegateListener(self);
    }
    return self;
}

-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) options
{
    // duongrs fix crash https://forum.unity.com/threads/ios-12-crash-audiotoolbox.719675/
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleInterruptionChangeToState:) name:AVAudioSessionInterruptionNotification object:nil];

    return YES;
}

- (void)handleInterruptionChangeToState:(NSNotification *)notification {
    NSDictionary *interuptionDict = notification.userInfo;
    NSInteger interuptionType = [[interuptionDict valueForKey:AVAudioSessionInterruptionTypeKey] integerValue];
 
    if (interuptionType == AVAudioSessionInterruptionTypeBegan){
        // NSLog(@"AVAudioSessionInterruptionTypeBegan");
        UnitySetAudioSessionActive(false);
        _pausedByInterruption = YES;
    }
    else if (interuptionType == AVAudioSessionInterruptionTypeEnded){
        // NSLog(@"AVAudioSessionInterruptionTypeEnded");
        if (_pausedByInterruption)
        {
            UnitySetAudioSessionActive(true);
            _pausedByInterruption = NO;
        }
    }
}

@end
