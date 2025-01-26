using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Endelways.ScreenManager
{
    public class ScreenService
    {
        private readonly List<Screen> _availableScreens;
        private readonly List<Screen> _openedScreens = new List<Screen>();
        private readonly List<Screen> _hidedScreens = new List<Screen>();
        private readonly List<Screen> _showedScreens = new List<Screen>();
        private readonly Transform _screenContainer;
        
        public ScreenService(Transform screenContainer)
        {
            _screenContainer = screenContainer;
            _availableScreens = Resources.LoadAll<Screen>("Screens").ToList();
        }

        public void Show<T>(IScreenOptions options = null) where T : Screen
        {
            var screen = _availableScreens.Find(screen => screen is T);
            if (screen == null)
                throw new InvalidDataException($"Screen of type {typeof(T)} doesn't exist");
            Show(screen, options);
        }

        private void Show(Screen screen, IScreenOptions options)
        {
            if (screen.IsReusableScreen)
            {
                ShowReusable(screen, options);
            }
            else
            {
                ShowSingle(screen, options);
            }
        }

        private void ShowReusable(Screen screen, IScreenOptions options)
        {
            if (IsScreenOpened(screen))
            {
                ShowPreviousOpenedScreen(screen);
            }
            else
            {
                InstantiateNewScreenAndOpen(screen, options);
            }
        }

        private void ShowSingle(Screen screen, IScreenOptions options)
        {
            if(IsScreenOpened(screen))
                return;
            InstantiateNewScreenAndOpen(screen, options);
        }

        private void ShowPreviousOpenedScreen(Screen screen)
        {
            if(IsScreenShowed(screen, out var showedScreen))
                return;
            _hidedScreens.Remove(showedScreen);
            _showedScreens.Add(showedScreen);
            showedScreen.gameObject.SetActive(true);
            showedScreen.OnDisplay();
            
        }

        private void InstantiateNewScreenAndOpen(Screen screen, IScreenOptions options)
        {
            var newScreen = Object.Instantiate(screen, _screenContainer);
            newScreen.OnOpened(options);
            newScreen.gameObject.SetActive(true);
            _openedScreens.Add(newScreen);
            _showedScreens.Add(newScreen);
        }
        
        public void Close<T>() where T : Screen
        {
            #if UNITY_EDITOR
                if (!EditorPlayMode.Active) return;
            #endif
            var screen = GetOpenedScreen(typeof(T));
            if (screen == null )
                throw new InvalidDataException($"Screen of type {typeof(T)} doesn't opened");
            Close(screen);
        }
        
        private void Close(Screen screen)
        {
            Object.Destroy(screen.gameObject);
            screen.OnClosed();
            if (_showedScreens.Contains(screen))
                _showedScreens.Remove(screen);
            if (_hidedScreens.Contains(screen))
                _hidedScreens.Remove(screen);
            _openedScreens.Remove(screen);
        }

        public void Hide<T>() where T : Screen
        {
            var screen = GetOpenedScreen(typeof(T));
            if (screen == null)
                throw new InvalidDataException($"Screen of type {typeof(T)} doesn't opened");
            if(!screen.IsReusableScreen)
                Close(screen);
            screen.gameObject.SetActive(false);
            screen.OnHide();
            _showedScreens.Remove(screen);
            _hidedScreens.Add(screen);
        }

        private Screen GetOpenedScreen(Type T)
        {
            return T.BaseType != typeof(Screen) ? null : _openedScreens.Find(scr => scr.GetType() == T);
        }

        private Screen GetOpenedScreen(Screen screen)
        {
            return GetOpenedScreen(screen.GetType());
        }
        
        private Screen GetShowedScreen(Screen screen)
        {
            return _showedScreens.Find(scr => scr.GetType() == screen.GetType());
        }
        
        private bool IsScreenOpened(Screen screen)
        {
           return GetOpenedScreen(screen) != null;
        }
        
        private bool IsScreenOpened(Screen screen, out Screen openedScreen)
        {
            openedScreen = GetOpenedScreen(screen);
            return openedScreen != null;
        }
        
        private bool IsScreenShowed(Screen screen)
        {
            if (!screen.IsReusableScreen)
                return IsScreenOpened(screen);
            return GetShowedScreen(screen) != null;
        }
        
        private bool IsScreenShowed(Screen screen, out Screen showedScreen)
        {
            if (!screen.IsReusableScreen)
            {
                return IsScreenOpened(screen, out showedScreen);
            }
            showedScreen = GetShowedScreen(screen);
            return IsScreenShowed(screen);
        }

        public bool IsScreenOpened<T>() where T : Screen
        {
           return GetOpenedScreen(typeof(T)) != null;
        }
        
        public bool IsScreenShowed<T>() where T : Screen
        {
            
            var openedScreen = GetOpenedScreen(typeof(T));
            if (openedScreen == null) return false;
            if (!openedScreen.IsReusableScreen)
                return true;
            return GetShowedScreen(openedScreen) != null;
        }
    }
}