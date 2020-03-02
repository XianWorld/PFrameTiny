using Unity.Collections;
using Unity.Entities;
using Unity.Tiny.Input;
#if UNITY_DOTSPLAYER
using System;
#else
using UnityEngine;
#endif

namespace PFrame.Tiny
{
    public class InputUtil
    {
        public static int GetButtonDown(InputSystem input)
        {
            for(int i = 0;i<3; i++)
            {
                if(GetButtonDown(input, i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetButton(InputSystem input)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GetButton(input, i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetButtonUp(InputSystem input)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GetButtonUp(input, i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool GetButtonDown(InputSystem input, int button)
        {
            var ok = false;

#if UNITY_DOTSPLAYER
            var Input = input;//world.GetExistingSystem<InputSystem>();
            if (Input.IsTouchSupported() && Input.TouchCount() > 0)
            {
                if(button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchState.Began)
                        ok = true;
                }
            }
            else
            {
                ok = Input.GetMouseButtonDown(button);
            }

#else
            if (Input.touchSupported && Input.touchCount > 0)
            {
                if (button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                        ok = true;
                }
            }
            else
                ok = Input.GetMouseButtonDown(button);

#endif
            return ok;
        }

        public static bool GetButtonUp(InputSystem input, int button)
        {
            var ok = false;

#if UNITY_DOTSPLAYER
            var Input = input;//world.GetExistingSystem<InputSystem>();
            if (Input.IsTouchSupported() && Input.TouchCount() > 0)
            {
                if(button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchState.Ended)
                        ok = true;
                }
            }
            else
            {
                ok = Input.GetMouseButtonDown(button);
            }

#else
            if (Input.touchSupported && Input.touchCount > 0)
            {
                if(button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Ended)
                        ok = true;
                }
            }
            else
                ok = Input.GetMouseButtonUp(button);

#endif
            return ok;
        }

        public static bool GetButton(InputSystem input, int button)
        {
            var ok = false;

#if UNITY_DOTSPLAYER
            var Input = input;//world.GetExistingSystem<InputSystem>();
            if (Input.IsTouchSupported() && Input.TouchCount() > 0)
            {
                if(button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchState.Stationary)
                        ok = true;
                }
            }
            else
            {
                ok = Input.GetMouseButton(button);
            }

#else
            if (Input.touchSupported && Input.touchCount > 0)
            {
                if (button == 0)
                {
                    var touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Stationary)
                        ok = true;
                }
            }
            else
                ok = Input.GetMouseButton(button);

#endif
            return ok;
        }

    }
}