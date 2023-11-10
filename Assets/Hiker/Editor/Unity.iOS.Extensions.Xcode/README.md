#2020-06-04 - duongrs noted
Build bản custom XcodeAPI từ opensource proj https://bitbucket.org/Unity-Technologies/xcodeapi/
có tích hợp thêm code từ project: https://github.com/zeyangl/UnityAppNameLocalizationForIOS
để hỗ trợ localize tên app cho iOS khi build Xcode Proj từ Unity.
* Các file localize tên cho iOS ở trong thư mục Assets/NativeLocale/iOS
* Code tích hợp việc thêm các file localize vào bản build trong file HikerBuildEditor hàm PostProcessBuild iOS
