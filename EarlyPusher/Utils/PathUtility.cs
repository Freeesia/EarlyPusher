using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EarlyPusher.Utils
{
	public static class PathUtility
	{
		[DllImport( "shlwapi.dll", CharSet = CharSet.Auto )]
		private static extern bool PathRelativePathTo(
			 [Out] StringBuilder pszPath,
			 [In] string pszFrom,
			 [In] System.IO.FileAttributes dwAttrFrom,
			 [In] string pszTo,
			 [In] System.IO.FileAttributes dwAttrTo
		);

		[DllImport( "shlwapi.dll", CharSet = CharSet.Auto )]
		private static extern IntPtr PathCombine(
			[Out] StringBuilder lpszDest,
			string lpszDir,
			string lpszFile );

		/// <summary>
		/// 絶対パスから相対パスを取得します。
		/// </summary>
		/// <param name="basePath">基準とするフォルダのパス。</param>
		/// <param name="absolutePath">相対パス。</param>
		/// <returns>絶対パス。</returns>
		public static string GetRelativePath( string basePath, string absolutePath )
		{
			StringBuilder sb = new StringBuilder( 260 );
			bool res = PathRelativePathTo( sb,
				basePath, System.IO.FileAttributes.Directory,
				absolutePath, System.IO.FileAttributes.Normal );
			if( !res )
			{
				throw new Exception( "相対パスの取得に失敗しました。" );
			}
			return sb.ToString();
		}

		/// <summary>
		/// 相対パスから絶対パスを取得します。
		/// </summary>
		/// <param name="basePath">基準とするパス。</param>
		/// <param name="relativePath">相対パス。</param>
		/// <returns>絶対パス。</returns>
		public static string GetAbsolutePath( string basePath, string relativePath )
		{
			StringBuilder sb = new StringBuilder( 2048 );
			IntPtr res = PathCombine( sb, basePath, relativePath );
			if( res == IntPtr.Zero )
			{
				throw new Exception( "絶対パスの取得に失敗しました。" );
			}
			return sb.ToString();
		}
	}
}
