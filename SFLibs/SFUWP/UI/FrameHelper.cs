using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SFLibs.UWP.UI
{
    public static class FrameHelper
    {
        public static Frame GetRootFrame(bool createFrame = false)
        {
            var rootFrame = Window.Current.Content as Frame;

            // ウィンドウに既にコンテンツが表示されている場合は、アプリケーションの初期化を繰り返さずに、
            // ウィンドウがアクティブであることだけを確認してください
            if (rootFrame == null && createFrame)
            {
                // ナビゲーション コンテキストとして動作するフレームを作成し、最初のページに移動します
                rootFrame = new Frame();

                // フレームを現在のウィンドウに配置します
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }
    }
}
