using System;
using System.Collections.Generic;
using System.Text;
using Android.Views;

namespace APES.UI.XF.Droid
{
    class TouchListner : Java.Lang.Object, View.IOnTouchListener
    {
        readonly GestureDetector gestureListener;
        public TouchListner(GestureDetector gestureListener)
        {
            this.gestureListener = gestureListener;
        }
        public bool OnTouch(View? v, MotionEvent? e)
        {
            return gestureListener.OnTouchEvent(e);
        }
    }
}
