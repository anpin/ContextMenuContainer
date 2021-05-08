using System;
using Android.Views;
namespace APES.UI.XF.Droid
{
    class HoldGestureRecognizer : Java.Lang.Object, GestureDetector.IOnGestureListener //GestureDetector.SimpleOnGestureListener
	{
        readonly Action justDoIt;
        //readonly Action justDoItShortly;
		public HoldGestureRecognizer(Action toDo) //, Action click)
        {
            justDoIt = toDo;
			//justDoItShortly = click;
        }

        public  void OnLongPress(MotionEvent? e)
        {
            justDoIt();
        }
        //public override bool OnSingleTapUp(MotionEvent? e)
        //{
        //    return true;
        //}
        public  bool OnDown(MotionEvent? e)
        {
            return true;
        }


        public bool OnFling(MotionEvent? e1, MotionEvent? e2, float velocityX, float velocityY)
        {
            return false;
        }

        public bool OnScroll(MotionEvent? e1, MotionEvent? e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent? e)
        {

        }

        public bool OnSingleTapUp(MotionEvent? e)
        {
            //justDoItShortly();
            return false;
        }
    }
}
