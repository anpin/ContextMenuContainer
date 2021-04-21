using System;
using Android.Views;
namespace APES.UI.XF.Droid
{
    class HoldGestureRecognizer : Java.Lang.Object, GestureDetector.IOnGestureListener
    {
        readonly Action justDoIt;
        public HoldGestureRecognizer(Action toDo)
        {
            justDoIt = toDo;
        }

        public void OnLongPress(MotionEvent? e)
        {
            justDoIt();
        }

        public bool OnDown(MotionEvent? e)
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
            return false;
        }
    }
}
