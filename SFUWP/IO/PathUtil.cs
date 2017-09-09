using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SFLibs.UWP.IO
{
    public static class PathUtil
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Ansi)]
        private static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] FileAttributes dwAttrFrom,
             [In] string pszTo,
             [In] FileAttributes dwAttrTo
        );

        /// <summary>
        /// 絶対パスから相対パスを取得します。
        /// </summary>
        /// <param name="basePath">基準とするフォルダのパス。</param>
        /// <param name="absolutePath">相対パス。</param>
        /// <returns>絶対パス。</returns>
        public static string GetRelativePath(string basePath, string absolutePath)
        {
            var sb = new StringBuilder(260);
            if (!PathRelativePathTo(sb, basePath, FileAttributes.Directory, absolutePath, FileAttributes.Normal))
            {
                throw new Exception("相対パスの取得に失敗しました。");
            }
            return sb.ToString();
        }
    }
}
