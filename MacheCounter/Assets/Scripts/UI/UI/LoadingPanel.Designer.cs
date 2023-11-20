using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MacheCounter.UI
{
	// Generate Id:a24f82de-b224-4dd3-83ef-7b156aed7855
	public partial class LoadingPanel
	{
		public const string Name = "LoadingPanel";
		
		[SerializeField]
		public UnityEngine.UI.Slider LoadBar;
		
		private LoadingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LoadBar = null;
			
			mData = null;
		}
		
		public LoadingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LoadingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LoadingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
