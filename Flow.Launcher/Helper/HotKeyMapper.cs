﻿using Flow.Launcher.Infrastructure.Hotkey;
using Flow.Launcher.Infrastructure.UserSettings;
using System;
using NHotkey;
using NHotkey.Wpf;
using Flow.Launcher.Core.Resource;
using Flow.Launcher.ViewModel;
using Flow.Launcher.Core;
using ChefKeys;
using System.Globalization;

namespace Flow.Launcher.Helper;

internal static class HotKeyMapper
{
    private static Settings _settings;
    private static MainViewModel _mainViewModel;

    internal static void Initialize(MainViewModel mainVM)
    {
        _mainViewModel = mainVM;
        _settings = _mainViewModel.Settings;

        SetHotkey(_settings.Hotkey, OnToggleHotkey);
        LoadCustomPluginHotkey();
    }

    internal static void OnToggleHotkey(object sender, HotkeyEventArgs args)
    {
        if (!_mainViewModel.ShouldIgnoreHotkeys())
            _mainViewModel.ToggleFlowLauncher();
    }

    internal static void OnToggleHotkeyWithChefKeys()
    {
        if (!_mainViewModel.ShouldIgnoreHotkeys())
            _mainViewModel.ToggleFlowLauncher();
    }

    private static void SetHotkey(string hotkeyStr, EventHandler<HotkeyEventArgs> action)
    {
        if (hotkeyStr == "LWin" || hotkeyStr == "RWin")
        {
            SetWithChefKeys(hotkeyStr);
            return;
        }

        var hotkey = new HotkeyModel(hotkeyStr);
        SetHotkey(hotkey, action);
    }

    private static void SetWithChefKeys(string hotkeyStr)
    {
        ChefKeysManager.RegisterHotkey(hotkeyStr, hotkeyStr, OnToggleHotkeyWithChefKeys);
        ChefKeysManager.Start();
    }

    internal static void SetHotkey(HotkeyModel hotkey, EventHandler<HotkeyEventArgs> action)
    {
        string hotkeyStr = hotkey.ToString();

        if (hotkeyStr == "LWin" || hotkeyStr == "RWin")
        {
            SetWithChefKeys(hotkeyStr);
            return;
        }

        try
        {
            HotkeyManager.Current.AddOrReplace(hotkeyStr, hotkey.CharKey, hotkey.ModifierKeys, action);
        }
        catch (Exception)
        {
            string errorMsg = string.Format(InternationalizationManager.Instance.GetTranslation("registerHotkeyFailed"), hotkeyStr);
            string errorMsgTitle = InternationalizationManager.Instance.GetTranslation("MessageBoxTitle");
            MessageBoxEx.Show(errorMsg, errorMsgTitle);
        }
    }

    internal static void RemoveHotkey(string hotkeyStr)
    {
        if (hotkeyStr == "LWin" || hotkeyStr == "RWin")
        {
            RemoveWithChefKeys(hotkeyStr);
            return;
        }

        if (!string.IsNullOrEmpty(hotkeyStr))
        {
            HotkeyManager.Current.Remove(hotkeyStr);
        }
    }

    private static void RemoveWithChefKeys(string hotkeyStr)
    {
        ChefKeysManager.UnregisterHotkey(hotkeyStr);
        ChefKeysManager.Stop();
    }

    internal static void LoadCustomPluginHotkey()
    {
        if (_settings.CustomPluginHotkeys == null)
            return;

        foreach (CustomPluginHotkey hotkey in _settings.CustomPluginHotkeys)
        {
            SetCustomQueryHotkey(hotkey);
        }
    }

    internal static void SetCustomQueryHotkey(CustomPluginHotkey hotkey)
    {
        SetHotkey(hotkey.Hotkey, (s, e) =>
        {
            if (_mainViewModel.ShouldIgnoreHotkeys())
                return;

            _mainViewModel.Show();
            _mainViewModel.ChangeQueryText(hotkey.ActionKeyword, true);
        });
    }

    internal static bool CheckAvailability(HotkeyModel currentHotkey)
    {
        try
        {
            HotkeyManager.Current.AddOrReplace("HotkeyAvailabilityTest", currentHotkey.CharKey, currentHotkey.ModifierKeys, (sender, e) => { });

            return true;
        }
        catch
        {
        }
        finally
        {
            HotkeyManager.Current.Remove("HotkeyAvailabilityTest");
        }

        return false;
    }
}
