using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Saxxon.Foundations.Sdl3.Extensions;
using Saxxon.Foundations.Sdl3.Interop;

namespace Saxxon.Foundations.Sdl3;

/// <summary>
/// Provides an object-oriented interface for <see cref="SDL_Window"/>.
/// </summary>
[PublicAPI]
public static class Window
{
    public static unsafe void ClearComposition(
        this IntPtr<SDL_Window> window
    )
    {
        SDL_ClearComposition(window)
            .AssertSdlSuccess();
    }

    public static unsafe IntPtr<SDL_Window> Create(
        string title, SDL_Size size, SDL_WindowFlags flags
    )
    {
        using var titleStr = new UnmanagedString(title);
        return ((IntPtr<SDL_Window>)SDL_CreateWindow(titleStr, size.w, size.h, flags))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Window> CreatePopup(
        this IntPtr<SDL_Window> ptr,
        SDL_Rect rect,
        SDL_WindowFlags flags
    )
    {
        return ((IntPtr<SDL_Window>)SDL_CreatePopupWindow(ptr, rect.x, rect.y, rect.w, rect.h, flags))
            .AssertSdlNotNull();
    }

    public static unsafe (IntPtr<SDL_Window> Window, IntPtr<SDL_Renderer> Renderer) CreateWithRenderer(
        string title,
        int width,
        int height,
        SDL_WindowFlags windowFlags
    )
    {
        using var titleStr = new UnmanagedString(title);
        SDL_Window* window;
        SDL_Renderer* renderer;

        SDL_CreateWindowAndRenderer(
            titleStr,
            width,
            height,
            windowFlags,
            &window,
            &renderer
        ).AssertSdlSuccess();

        return ((IntPtr<SDL_Window>)window, (IntPtr<SDL_Renderer>)renderer);
    }

    public static unsafe IntPtr<SDL_Window> CreateWithProperties(SDL_PropertiesID props)
    {
        return ((IntPtr<SDL_Window>)SDL_CreateWindowWithProperties(props))
            .AssertSdlNotNull();
    }

    public static unsafe void Destroy(
        this IntPtr<SDL_Window> ptr
    )
    {
        SDL_DestroyWindow(ptr);
    }

    public static unsafe void DestroySurface(
        this IntPtr<SDL_Window> ptr
    )
    {
        SDL_DestroyWindowSurface(ptr);
    }

    public static unsafe void Flash(
        this IntPtr<SDL_Window> ptr,
        SDL_FlashOperation operation
    )
    {
        SDL_FlashWindow(ptr, operation);
    }

    public static unsafe IMemoryOwner<IntPtr<SDL_Window>> GetAll(
        this IntPtr<SDL_Window> ptr
    )
    {
        int count;
        var values = SDL_GetWindows(&count);

        return SdlMemoryManager.Owned(values, count);
    }

    public static unsafe (float MinAspect, float MaxAspect) GetAspectRatio(
        this IntPtr<SDL_Window> ptr
    )
    {
        float minAspect, maxAspect;
        SDL_GetWindowAspectRatio(ptr, &minAspect, &maxAspect)
            .AssertSdlSuccess();
        return (minAspect, maxAspect);
    }

    public static unsafe (int Top, int Left, int Bottom, int Right) GetBorderSize(
        this IntPtr<SDL_Window> ptr
    )
    {
        int top, left, bottom, right;
        SDL_GetWindowBordersSize(ptr, &top, &left, &bottom, &right)
            .AssertSdlSuccess();
        return (top, left, bottom, right);
    }

    public static unsafe SDL_DisplayID GetDisplay(
        this IntPtr<SDL_Window> ptr
    )
    {
        return SDL_GetDisplayForWindow(ptr);
    }

    public static unsafe float GetDisplayScale(
        this IntPtr<SDL_Window> ptr
    )
    {
        return SDL_GetWindowDisplayScale(ptr);
    }

    public static unsafe SDL_WindowFlags GetFlags(
        this IntPtr<SDL_Window> ptr
    )
    {
        return SDL_GetWindowFlags(ptr);
    }

    public static unsafe IntPtr<SDL_Window> GetFromId(
        SDL_WindowID id
    )
    {
        return ((IntPtr<SDL_Window>)SDL_GetWindowFromID(id))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_DisplayMode> GetFullscreenMode(
        this IntPtr<SDL_Window> ptr
    )
    {
        return ((IntPtr<SDL_DisplayMode>)SDL_GetWindowFullscreenMode(ptr))
            .AssertSdlNotNull();
    }

    public static unsafe IntPtr<SDL_Window> GetGrabbed(
        this IntPtr<SDL_Window> ptr
    )
    {
        return SDL_GetGrabbedWindow();
    }

    public static unsafe IMemoryOwner<byte> GetIccProfile(
        this IntPtr<SDL_Window> ptr
    )
    {
        UIntPtr size;

        var profile = (IntPtr<byte>)SDL_GetWindowICCProfile(ptr, &size)
            .AssertSdlNotNull();

        return SdlMemoryManager.Owned(profile, (int)size);
    }

    public static unsafe SDL_WindowID GetId(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowID(ptr);
    }

    public static unsafe bool GetKeyboardGrab(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowKeyboardGrab(ptr);
    }

    public static unsafe (int W, int H) GetMaximumSize(this IntPtr<SDL_Window> ptr)
    {
        int w, h;
        SDL_GetWindowMaximumSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe (int W, int H) GetMinimumSize(this IntPtr<SDL_Window> ptr)
    {
        int w, h;
        SDL_GetWindowMinimumSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return (w, h);
    }

    public static unsafe bool GetMouseGrab(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowMouseGrab(ptr);
    }

    public static unsafe SDL_Rect? GetMouseRect(this IntPtr<SDL_Window> ptr)
    {
        var result = SDL_GetWindowMouseRect(ptr);
        return result == null ? null : *(SDL_Rect*)&result;
    }

    public static unsafe float GetOpacity(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowOpacity(ptr);
    }

    public static unsafe IntPtr<SDL_Window> GetParent(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowParent(ptr);
    }

    public static unsafe float GetPixelDensity(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowPixelDensity(ptr);
    }

    public static unsafe SDL_PixelFormat GetPixelFormat(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowPixelFormat(ptr);
    }

    public static unsafe SDL_Point GetPosition(this IntPtr<SDL_Window> ptr)
    {
        int x, y;
        SDL_GetWindowPosition(ptr, &x, &y)
            .AssertSdlSuccess();
        return Point.Create(x, y);
    }

    public static unsafe SDL_PropertiesID GetProperties(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowProperties(ptr);
    }

    public static unsafe SDL_ProgressState GetProgressState(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowProgressState(ptr);
    }

    public static unsafe float GetProgressValue(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowProgressValue(ptr);
    }

    public static unsafe SDL_Rect GetSafeArea(this IntPtr<SDL_Window> ptr)
    {
        SDL_Rect result;
        SDL_GetWindowSafeArea(ptr, &result)
            .AssertSdlSuccess();
        return *&result;
    }

    public static unsafe SDL_Size GetSize(this IntPtr<SDL_Window> ptr)
    {
        int w, h;
        SDL_GetWindowSize(ptr, &w, &h)
            .AssertSdlSuccess();
        return Size.Create(w, h);
    }

    public static unsafe SDL_Size GetSizeInPixels(this IntPtr<SDL_Window> ptr)
    {
        int w, h;
        SDL_GetWindowSizeInPixels(ptr, &w, &h)
            .AssertSdlSuccess();
        return Size.Create(w, h);
    }

    public static unsafe IntPtr<SDL_Surface> GetSurface(this IntPtr<SDL_Window> ptr)
    {
        return ((IntPtr<SDL_Surface>)SDL_GetWindowSurface(ptr))
            .AssertSdlNotNull();
    }

    public static unsafe int GetSurfaceVSync(this IntPtr<SDL_Window> ptr)
    {
        int vSync;
        SDL_GetWindowSurfaceVSync(ptr, &vSync)
            .AssertSdlSuccess();
        return vSync;
    }

    public static unsafe (SDL_Rect Rect, int Cursor) GetTextInputArea(
        this IntPtr<SDL_Window> window
    )
    {
        SDL_Rect rect;
        int cursor;
        SDL_GetTextInputArea(window, &rect, &cursor)
            .AssertSdlSuccess();
        return (rect, cursor);
    }

    public static unsafe string GetTitle(this IntPtr<SDL_Window> ptr)
    {
        return SDL_GetWindowTitle(ptr) ?? "";
    }
    
    public static IntPtr<SDL_Window> GetWindow(this SDL_WindowID id) =>
        GetFromId(id);

    public static unsafe bool HasSurface(this IntPtr<SDL_Window> ptr)
    {
        return SDL_WindowHasSurface(ptr);
    }

    public static unsafe void Hide(this IntPtr<SDL_Window> ptr)
    {
        SDL_HideWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe bool IsRelativeMouseMode(
        this IntPtr<SDL_Window> window
    )
    {
        return SDL_GetWindowRelativeMouseMode(window);
    }

    public static unsafe bool IsScreenKeyboardShown(
        this IntPtr<SDL_Window> window
    )
    {
        return SDL_ScreenKeyboardShown(window);
    }

    public static unsafe bool IsTextInputActive(
        this IntPtr<SDL_Window> window
    )
    {
        return SDL_TextInputActive(window);
    }

    public static unsafe void Maximize(this IntPtr<SDL_Window> ptr)
    {
        SDL_MaximizeWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void Minimize(this IntPtr<SDL_Window> ptr)
    {
        SDL_MinimizeWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void Raise(this IntPtr<SDL_Window> ptr)
    {
        SDL_RaiseWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void Restore(this IntPtr<SDL_Window> ptr)
    {
        SDL_RestoreWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void SetAlwaysOnTop(this IntPtr<SDL_Window> ptr, bool onTop)
    {
        SDL_SetWindowAlwaysOnTop(ptr, onTop)
            .AssertSdlSuccess();
    }

    public static unsafe void SetAspectRatio(this IntPtr<SDL_Window> ptr, float minAspect, float maxAspect)
    {
        SDL_SetWindowAspectRatio(ptr, minAspect, maxAspect)
            .AssertSdlSuccess();
    }

    public static unsafe void SetBordered(this IntPtr<SDL_Window> ptr, bool bordered)
    {
        SDL_SetWindowBordered(ptr, bordered)
            .AssertSdlSuccess();
    }

    public static unsafe void SetFocusable(this IntPtr<SDL_Window> ptr, bool focusable)
    {
        SDL_SetWindowFocusable(ptr, focusable)
            .AssertSdlSuccess();
    }

    public static unsafe void SetFullscreen(this IntPtr<SDL_Window> ptr, bool fullscreen)
    {
        SDL_SetWindowFullscreen(ptr, fullscreen)
            .AssertSdlSuccess();
    }

    public static unsafe void SetFullscreenMode(this IntPtr<SDL_Window> ptr, IntPtr<SDL_DisplayMode> displayMode)
    {
        SDL_SetWindowFullscreenMode(ptr, displayMode)
            .AssertSdlSuccess();
    }

    public delegate SDL_HitTestResult HitTestDelegate(SDL_Point point);

    private static readonly Dictionary<IntPtr<SDL_Window>, IntPtr> HitTestUserData = [];

    public static unsafe void SetHitTest(this IntPtr<SDL_Window> ptr, HitTestDelegate? callback)
    {
        if (HitTestUserData.Remove(ptr, out var existing))
            UserDataStore.Remove(existing);

        if (callback == null)
            return;

        var userData = UserDataStore.Add(callback);
        HitTestUserData.Add(ptr, userData);
        SDL_SetWindowHitTest(ptr, &Execute, userData);

        return;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
        static SDL_HitTestResult Execute(SDL_Window* window, SDL_Point* point, IntPtr userData)
        {
            return UserDataStore.TryGet<HitTestDelegate>(userData, out var target)
                ? target!.Invoke(*point)
                : default;
        }
    }

    public static unsafe void SetIcon(this IntPtr<SDL_Window> ptr, IntPtr<SDL_Surface> icon)
    {
        SDL_SetWindowIcon(ptr, icon)
            .AssertSdlSuccess();
    }

    public static unsafe void SetKeyboardGrab(this IntPtr<SDL_Window> ptr, bool grab)
    {
        SDL_SetWindowKeyboardGrab(ptr, grab)
            .AssertSdlSuccess();
    }

    public static unsafe void SetMaximumSize(this IntPtr<SDL_Window> ptr, SDL_Size size)
    {
        SDL_SetWindowMaximumSize(ptr, size.w, size.h)
            .AssertSdlSuccess();
    }

    public static unsafe void SetMinimumSize(this IntPtr<SDL_Window> ptr, SDL_Size size)
    {
        SDL_SetWindowMinimumSize(ptr, size.w, size.h)
            .AssertSdlSuccess();
    }

    public static unsafe void SetModal(this IntPtr<SDL_Window> ptr, bool modal)
    {
        SDL_SetWindowModal(ptr, modal)
            .AssertSdlSuccess();
    }

    public static unsafe void SetMouseGrab(this IntPtr<SDL_Window> ptr, bool grab)
    {
        SDL_SetWindowMouseGrab(ptr, grab)
            .AssertSdlSuccess();
    }

    public static unsafe void SetMouseRect(this IntPtr<SDL_Window> ptr, SDL_Rect? rect)
    {
        SDL_SetWindowMouseRect(ptr, rect is { } r ? &r : null)
            .AssertSdlSuccess();
    }

    public static unsafe void SetOpacity(this IntPtr<SDL_Window> ptr, float opacity)
    {
        SDL_SetWindowOpacity(ptr, opacity)
            .AssertSdlSuccess();
    }

    public static unsafe void SetParent(this IntPtr<SDL_Window> ptr, IntPtr<SDL_Window> window)
    {
        SDL_SetWindowParent(ptr, window)
            .AssertSdlSuccess();
    }

    public static unsafe void SetPosition(this IntPtr<SDL_Window> ptr, int x, int y)
    {
        SDL_SetWindowPosition(ptr, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe void SetPosition(this IntPtr<SDL_Window> ptr, SDL_Point position)
    {
        SDL_SetWindowPosition(ptr, position.x, position.y)
            .AssertSdlSuccess();
    }

    public static unsafe void SetProgressState(this IntPtr<SDL_Window> ptr, SDL_ProgressState value)
    {
        SDL_SetWindowProgressState(ptr, value)
            .AssertSdlSuccess();
    }

    public static unsafe void SetProgressValue(this IntPtr<SDL_Window> ptr, float value)
    {
        SDL_SetWindowProgressValue(ptr, value)
            .AssertSdlSuccess();
    }

    public static unsafe void SetRelativeMouseMode(
        this IntPtr<SDL_Window> window,
        bool enabled
    )
    {
        SDL_SetWindowRelativeMouseMode(window, enabled)
            .AssertSdlSuccess();
    }

    public static unsafe void SetResizable(this IntPtr<SDL_Window> ptr, bool resizable)
    {
        SDL_SetWindowResizable(ptr, resizable)
            .AssertSdlSuccess();
    }

    public static unsafe void SetShape(this IntPtr<SDL_Window> ptr, IntPtr<SDL_Surface> shape)
    {
        SDL_SetWindowShape(ptr, shape)
            .AssertSdlSuccess();
    }

    public static unsafe void SetSize(this IntPtr<SDL_Window> ptr, int w, int h)
    {
        SDL_SetWindowSize(ptr, w, h)
            .AssertSdlSuccess();
    }

    public static unsafe void SetSize(this IntPtr<SDL_Window> ptr, SDL_Size size)
    {
        SDL_SetWindowSize(ptr, size.w, size.h)
            .AssertSdlSuccess();
    }

    public static unsafe void SetSurfaceVSync(this IntPtr<SDL_Window> ptr, int vSync)
    {
        SDL_SetWindowSurfaceVSync(ptr, vSync)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTextInputArea(
        this IntPtr<SDL_Window> window,
        SDL_Rect? rect,
        int cursor
    )
    {
        SDL_SetTextInputArea(window, rect is { } dr ? &dr : null, cursor)
            .AssertSdlSuccess();
    }

    public static unsafe void SetTitle(this IntPtr<SDL_Window> ptr, string title)
    {
        using var titleStr = new UnmanagedString(title);
        SDL_SetWindowTitle(ptr, titleStr)
            .AssertSdlSuccess();
    }

    public static unsafe void Show(this IntPtr<SDL_Window> ptr)
    {
        SDL_ShowWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void ShowSystemMenu(this IntPtr<SDL_Window> ptr, int x, int y)
    {
        SDL_ShowWindowSystemMenu(ptr, x, y)
            .AssertSdlSuccess();
    }

    public static unsafe void ShowSystemMenu(this IntPtr<SDL_Window> ptr, SDL_Point point)
    {
        SDL_ShowWindowSystemMenu(ptr, point.x, point.y)
            .AssertSdlSuccess();
    }

    public static unsafe void StartTextInput(
        this IntPtr<SDL_Window> window
    )
    {
        SDL_StartTextInput(window)
            .AssertSdlSuccess();
    }

    public static unsafe void StartTextInputWithProperties(
        this IntPtr<SDL_Window> window,
        SDL_PropertiesID props
    )
    {
        SDL_StartTextInputWithProperties(window, props)
            .AssertSdlSuccess();
    }

    public static unsafe void StopTextInput(
        this IntPtr<SDL_Window> window
    )
    {
        SDL_StopTextInput(window)
            .AssertSdlSuccess();
    }

    public static unsafe void Sync(this IntPtr<SDL_Window> ptr)
    {
        SDL_SyncWindow(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void UpdateSurface(this IntPtr<SDL_Window> ptr)
    {
        SDL_UpdateWindowSurface(ptr)
            .AssertSdlSuccess();
    }

    public static unsafe void UpdateSurfaceRects(this IntPtr<SDL_Window> ptr, ReadOnlySpan<SDL_Rect> rects)
    {
        fixed (SDL_Rect* data = rects)
        {
            SDL_UpdateWindowSurfaceRects(ptr, data, rects.Length)
                .AssertSdlSuccess();
        }
    }

    public static unsafe void WarpMouse(
        this IntPtr<SDL_Window> window,
        float x,
        float y
    )
    {
        SDL_WarpMouseInWindow(window, x, y);
    }

    public static unsafe void WarpMouse(
        this IntPtr<SDL_Window> window,
        SDL_FPoint point
    )
    {
        SDL_WarpMouseInWindow(window, point.x, point.y);
    }
}