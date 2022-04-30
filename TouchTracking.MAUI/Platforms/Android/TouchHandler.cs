﻿using Android.Views;
using System;
using System.Collections.Generic;

namespace TouchTracking.Droid
{
    public class TouchHandler : TouchHandlerBase<Android.Views.View>
    {
        Android.Views.View _view;
        bool _capture;
        Func<double, double> _fromPixels;
        int[] _locationOnScreen = new int[2];

        static Dictionary<Android.Views.View, TouchHandler> _viewDictionary =
            new Dictionary<Android.Views.View, TouchHandler>();

        static Dictionary<int, TouchHandler> _idToTouchHandlerDictionary =
            new Dictionary<int, TouchHandler>();

        private float _lastX;
        private float _lastY;
        private float _slop;
        private bool _moveInProgress;

        public bool UseTouchSlop { get; set; }

        public override void RegisterEvents(Android.Views.View view)
        {
            _view = view;
            _slop = ViewConfiguration.Get(view.Context).ScaledTouchSlop;

            if (view != null)
            {
                _viewDictionary.Add(view, this);

                _fromPixels = view.Context.FromPixels;

                // Set event handler on View
                view.Touch += OnTouch;
            }
        }

        public override void UnregisterEvents(Android.Views.View view)
        {
            try
            {
                view.GetHashCode();
            }
            catch (ObjectDisposedException) //view can be already disposed and we have no other way to remove it from dictionary
            {
                var newDictionary = new Dictionary<Android.Views.View, TouchHandler>();
                foreach (KeyValuePair<Android.Views.View, TouchHandler> item in _viewDictionary)
                {
                    try
                    {
                        newDictionary[item.Key] = item.Value;
                    }
                    catch (ObjectDisposedException)
                    {
                        continue;
                    }
                }
                _viewDictionary = newDictionary;
                return;
            }
            if (_viewDictionary.ContainsKey(view))
            {
                _viewDictionary.Remove(view);
                view.Touch -= OnTouch;
            }
        }

        private TouchTrackingPoint GetScreenPointerCoordinates(
            int[] screenLocation, MotionEvent motionEvent, int pointerIndex)
        {
            var screenPointerCoords =
                new TouchTrackingPoint(
                    screenLocation[0] + motionEvent.GetX(pointerIndex),
                    screenLocation[1] + motionEvent.GetY(pointerIndex));

            return screenPointerCoords;
        }

        private void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(_locationOnScreen);
            TouchTrackingPoint screenPointerCoords =
                GetScreenPointerCoordinates(_locationOnScreen, motionEvent, pointerIndex);


            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);

                    _idToTouchHandlerDictionary.Add(id, this);

                    _capture = Capture;

                    if (UseTouchSlop)
                    {
                        _lastX = args.Event.GetX();
                        _lastY = args.Event.GetY();
                    }
                    break;

                case MotionEventActions.Move:
                    if (motionEvent.PointerCount == 1 && UseTouchSlop)
                    {
                        id = motionEvent.GetPointerId(0);
                        senderView.GetLocationOnScreen(_locationOnScreen);

                        if (!_moveInProgress)
                        {
                            var x = args.Event.GetX();
                            var y = args.Event.GetY();

                            var xDiff = Math.Abs(_lastX - x);
                            var yDiff = Math.Abs(_lastY - y);

                            if (xDiff > _slop || yDiff > _slop)
                            {
                                _moveInProgress = true;
                            }
                        }

                        if (_moveInProgress)
                        {
                            screenPointerCoords = GetScreenPointerCoordinates(_locationOnScreen, motionEvent, pointerIndex);
                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }

                        break;
                    }

                    _moveInProgress = false;
                    _lastX = 0;
                    _lastY = 0;

                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_capture)
                        {
                            senderView.GetLocationOnScreen(_locationOnScreen);

                            screenPointerCoords = GetScreenPointerCoordinates(_locationOnScreen, motionEvent, pointerIndex);
                            FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, screenPointerCoords);

                            if (_idToTouchHandlerDictionary[id] != null)
                            {
                                FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                            }
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);

                        if (_idToTouchHandlerDictionary[id] != null)
                        {
                            FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                        }
                    }
                    _idToTouchHandlerDictionary.Remove(id);

                    _moveInProgress = false;
                    _lastX = 0;
                    _lastY = 0;
                    break;

                case MotionEventActions.Cancel:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, screenPointerCoords, false);
                    }
                    else
                    {
                        if (_idToTouchHandlerDictionary[id] != null)
                        {
                            FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Cancelled, screenPointerCoords, false);
                        }
                    }
                    _idToTouchHandlerDictionary.Remove(id);

                    _moveInProgress = false;
                    _lastX = 0;
                    _lastY = 0;
                    break;
            }
        }

        private void CheckForBoundaryHop(int id, TouchTrackingPoint pointerLocation)
        {
            TouchHandler touchEffectHit = null;

            foreach (Android.Views.View view in _viewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(_locationOnScreen);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }
                TouchTrackingRect viewRect = new TouchTrackingRect(_locationOnScreen[0], _locationOnScreen[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation))
                {
                    touchEffectHit = _viewDictionary[view];
                }
            }

            if (touchEffectHit != _idToTouchHandlerDictionary[id])
            {
                if (_idToTouchHandlerDictionary[id] != null)
                {
                    FireEvent(_idToTouchHandlerDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }
                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }
                _idToTouchHandlerDictionary[id] = touchEffectHit;
            }
        }

        private void FireEvent(TouchHandler touchEffect, int id, TouchActionType actionType, TouchTrackingPoint pointerLocation, bool isInContact)
        {
            // Get the location of the pointer within the view
            touchEffect._view.GetLocationOnScreen(_locationOnScreen);
            double x = pointerLocation.X - _locationOnScreen[0];
            double y = pointerLocation.Y - _locationOnScreen[1];
            TouchTrackingPoint point = new TouchTrackingPoint((float)_fromPixels(x), (float)_fromPixels(y));

            // Call the method
            OnTouchAction(touchEffect._view,
                new TouchActionEventArgs(id, actionType, point, isInContact));
        }
    }
}
