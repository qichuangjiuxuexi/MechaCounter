using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MacheCounter.UI
{
	public class LoadingPanelData : UIPanelData
	{
	}
	public partial class LoadingPanel : UIPanel
	{
		private float progress = 0f;
		private float Timer = 0f;
		private float TargetTime = 0f;
		private float mStartValue = 0f;
		private float mEndValue = 0f;
		private EasyEvent mUpdateEvent;
		
		public Action mCallback;



		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LoadingPanelData ?? new LoadingPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			// LoadBar.value = 0;
		}
		
		protected override void OnShow()
		{
			mCallback = () =>
			{
				SetProgress(1, 2);
			};
			LoadBar.value = 0f;
			//进度先到30%开始读表
			ActionKit.Sequence()
				.Callback(() => Debug.Log("开始加载喽！！！"))
				.Callback(() => SetProgress(0.3f, 1))
				.Delay(2)
				.Callback(() =>
				{
					//实际上图标的callback,最后触发满进度逻辑
					mCallback?.Invoke();
				}).Start(this);
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void SetProgress(float progress, float time)
		{
			TargetTime = time;
			mEndValue = progress;
			mStartValue = this.progress;
			GameObject actionNode = new GameObject();
			actionNode.InstantiateWithParent(transform);

			ActionKit.OnUpdate.Register(() =>
			{
				Timer += Time.deltaTime;
				if (Timer < TargetTime)
				{
					float percent = Timer / TargetTime;
					float tweenValue = Mathf.Lerp(mStartValue, mEndValue, percent);
					SetPercentImmediate(tweenValue);
				}
				else
				{
					SetPercentImmediate(mEndValue);
					actionNode.DestroySelfGracefully();
				}
			}).UnRegisterWhenGameObjectDestroyed(actionNode);
		}
		
		public void SetPercentImmediate(float percent)
		{
			if (LoadBar != null)
			{
				LoadBar.value = percent;
			}
		}
	}
}
