// SPDX-License-Identifier: BSD-2-Clause

using System.Collections.Generic;
using System.Text;
using SDL3;

namespace RuneUO.Input
{
    internal static class KeysTranslator
    {
        private static readonly Dictionary<SDL.SDL_Keycode, string> _keys = new Dictionary<SDL.SDL_Keycode, string>
        {
            { SDL.SDL_Keycode.SDLK_UNKNOWN, "None" },
            { SDL.SDL_Keycode.SDLK_BACKSPACE, "Backspace" },
            { SDL.SDL_Keycode.SDLK_TAB, "Tab" },
            { SDL.SDL_Keycode.SDLK_RETURN, "Return" },
            { SDL.SDL_Keycode.SDLK_ESCAPE, "Esc" },
            { SDL.SDL_Keycode.SDLK_SPACE, "Space" },
            { SDL.SDL_Keycode.SDLK_EXCLAIM, "!" },
            { SDL.SDL_Keycode.SDLK_DOLLAR, "$" },
            { SDL.SDL_Keycode.SDLK_PERCENT, "%" },
            { SDL.SDL_Keycode.SDLK_APOSTROPHE, "'" },
            { SDL.SDL_Keycode.SDLK_ASTERISK, "*" },
            { SDL.SDL_Keycode.SDLK_PLUS, "+" },
            { SDL.SDL_Keycode.SDLK_COMMA, "," },
            { SDL.SDL_Keycode.SDLK_MINUS, "-" },
            { SDL.SDL_Keycode.SDLK_PERIOD, "." },
            { SDL.SDL_Keycode.SDLK_SLASH, "/" },
            { SDL.SDL_Keycode.SDLK_0, "0" },
            { SDL.SDL_Keycode.SDLK_1, "1" },
            { SDL.SDL_Keycode.SDLK_2, "2" },
            { SDL.SDL_Keycode.SDLK_3, "3" },
            { SDL.SDL_Keycode.SDLK_4, "4" },
            { SDL.SDL_Keycode.SDLK_5, "5" },
            { SDL.SDL_Keycode.SDLK_6, "6" },
            { SDL.SDL_Keycode.SDLK_7, "7" },
            { SDL.SDL_Keycode.SDLK_8, "8" },
            { SDL.SDL_Keycode.SDLK_9, "9" },
            { SDL.SDL_Keycode.SDLK_COLON, ":" },
            { SDL.SDL_Keycode.SDLK_SEMICOLON, ";" },
            { SDL.SDL_Keycode.SDLK_LESS, "<" },
            { SDL.SDL_Keycode.SDLK_EQUALS, "=" },
            { SDL.SDL_Keycode.SDLK_GREATER, ">" },
            { SDL.SDL_Keycode.SDLK_QUESTION, "?" },
            { SDL.SDL_Keycode.SDLK_LEFTBRACKET, "[" },
            { SDL.SDL_Keycode.SDLK_BACKSLASH, "\\" },
            { SDL.SDL_Keycode.SDLK_RIGHTBRACKET, "]" },
            { SDL.SDL_Keycode.SDLK_CARET, "-" },
            { SDL.SDL_Keycode.SDLK_UNDERSCORE, "_" },
            { SDL.SDL_Keycode.SDLK_GRAVE, "BACKQUOTE" }, //  = 96, // 0x00000060
            { SDL.SDL_Keycode.SDLK_A, "A" },
            { SDL.SDL_Keycode.SDLK_B, "B" },
            { SDL.SDL_Keycode.SDLK_C, "C" },
            { SDL.SDL_Keycode.SDLK_D, "D" },
            { SDL.SDL_Keycode.SDLK_E, "E" },
            { SDL.SDL_Keycode.SDLK_F, "F" },
            { SDL.SDL_Keycode.SDLK_G, "G" },
            { SDL.SDL_Keycode.SDLK_H, "H" },
            { SDL.SDL_Keycode.SDLK_I, "I" },
            { SDL.SDL_Keycode.SDLK_J, "J" },
            { SDL.SDL_Keycode.SDLK_K, "K" },
            { SDL.SDL_Keycode.SDLK_L, "L" },
            { SDL.SDL_Keycode.SDLK_M, "M" },
            { SDL.SDL_Keycode.SDLK_N, "N" },
            { SDL.SDL_Keycode.SDLK_O, "O" },
            { SDL.SDL_Keycode.SDLK_P, "P" },
            { SDL.SDL_Keycode.SDLK_Q, "Q" },
            { SDL.SDL_Keycode.SDLK_R, "R" },
            { SDL.SDL_Keycode.SDLK_S, "S" },
            { SDL.SDL_Keycode.SDLK_T, "T" },
            { SDL.SDL_Keycode.SDLK_U, "U" },
            { SDL.SDL_Keycode.SDLK_V, "V" },
            { SDL.SDL_Keycode.SDLK_W, "W" },
            { SDL.SDL_Keycode.SDLK_X, "X" },
            { SDL.SDL_Keycode.SDLK_Y, "Y" },
            { SDL.SDL_Keycode.SDLK_Z, "Z" },
            { SDL.SDL_Keycode.SDLK_DELETE, "DEL" },
            { SDL.SDL_Keycode.SDLK_CAPSLOCK, "CAPS" },
            { SDL.SDL_Keycode.SDLK_F1, "F1" },
            { SDL.SDL_Keycode.SDLK_F2, "F2" },
            { SDL.SDL_Keycode.SDLK_F3, "F3" },
            { SDL.SDL_Keycode.SDLK_F4, "F4" },
            { SDL.SDL_Keycode.SDLK_F5, "F5" },
            { SDL.SDL_Keycode.SDLK_F6, "F6" },
            { SDL.SDL_Keycode.SDLK_F7, "F7" },
            { SDL.SDL_Keycode.SDLK_F8, "F8" },
            { SDL.SDL_Keycode.SDLK_F9, "F9" },
            { SDL.SDL_Keycode.SDLK_F10, "F10" },
            { SDL.SDL_Keycode.SDLK_F11, "F11" },
            { SDL.SDL_Keycode.SDLK_F12, "F12" },
            { SDL.SDL_Keycode.SDLK_PRINTSCREEN, "Print" },
            { SDL.SDL_Keycode.SDLK_SCROLLLOCK, "Lock" },
            { SDL.SDL_Keycode.SDLK_PAUSE, "Pause" },
            { SDL.SDL_Keycode.SDLK_INSERT, "Ins" },
            { SDL.SDL_Keycode.SDLK_HOME, "Home" },
            { SDL.SDL_Keycode.SDLK_PAGEUP, "PG UP" },
            { SDL.SDL_Keycode.SDLK_END, "END" },
            { SDL.SDL_Keycode.SDLK_PAGEDOWN, "PG DOWN" },
            { SDL.SDL_Keycode.SDLK_RIGHT, "Right" },
            { SDL.SDL_Keycode.SDLK_LEFT, "Left" },
            { SDL.SDL_Keycode.SDLK_DOWN, "Down" },
            { SDL.SDL_Keycode.SDLK_UP, "Up" },
            { SDL.SDL_Keycode.SDLK_KP_DIVIDE, "/" },
            { SDL.SDL_Keycode.SDLK_KP_MULTIPLY, "*" },
            { SDL.SDL_Keycode.SDLK_KP_MINUS, "-" },
            { SDL.SDL_Keycode.SDLK_KP_PLUS, "+" },
            { SDL.SDL_Keycode.SDLK_KP_ENTER, "Enter" },
            { SDL.SDL_Keycode.SDLK_KP_1, "NUM 1" },
            { SDL.SDL_Keycode.SDLK_KP_2, "NUM 2" },
            { SDL.SDL_Keycode.SDLK_KP_3, "NUM 3" },
            { SDL.SDL_Keycode.SDLK_KP_4, "NUM 4" },
            { SDL.SDL_Keycode.SDLK_KP_5, "NUM 5" },
            { SDL.SDL_Keycode.SDLK_KP_6, "NUM 6" },
            { SDL.SDL_Keycode.SDLK_KP_7, "NUM 7" },
            { SDL.SDL_Keycode.SDLK_KP_8, "NUM 8" },
            { SDL.SDL_Keycode.SDLK_KP_9, "NUM 9" },
            { SDL.SDL_Keycode.SDLK_KP_0, "NUM 0" },
            { SDL.SDL_Keycode.SDLK_KP_PERIOD, "." },
            { SDL.SDL_Keycode.SDLK_KP_EQUALS, "=" }
        };



        public static string TryGetKey(SDL.SDL_Keycode key, SDL.SDL_Keymod mod = SDL.SDL_Keymod.SDL_KMOD_NONE)
        {
            if (_keys.TryGetValue(key, out string value))
            {
                StringBuilder sb = new StringBuilder();

                AddPrefix(sb, mod);

                sb.Append(value);

                return sb.ToString();
            }
            else
            {
                string sKey;
                if ((int)key < 256)
                    sKey = $"{(char)key}";
                else
                    sKey = $"{key}";
                _keys.Add(key, sKey);
                StringBuilder sb = new StringBuilder();
                AddPrefix(sb, mod);
                sb.Append(sKey);
                return sb.ToString();
            }
        }

        public static string GetMouseButton(MouseButtonType button, SDL.SDL_Keymod mod)
        {
            StringBuilder sb = new StringBuilder();

            AddPrefix(sb, mod);

            sb.Append(button.ToString());

            return sb.ToString();
        }

        public static string GetMouseWheel(bool wheelUp, SDL.SDL_Keymod mod)
        {
            StringBuilder sb = new StringBuilder();

            AddPrefix(sb, mod);

            sb.Append(wheelUp ? "WheelUp" : "WheelDown");

            return sb.ToString();
        }

        private static void AddPrefix(StringBuilder sb, SDL.SDL_Keymod mod)
        {
            bool isshift = (mod & SDL.SDL_Keymod.SDL_KMOD_SHIFT) != SDL.SDL_Keymod.SDL_KMOD_NONE;
            bool isctrl = (mod & SDL.SDL_Keymod.SDL_KMOD_CTRL) != SDL.SDL_Keymod.SDL_KMOD_NONE;
            bool isalt = (mod & SDL.SDL_Keymod.SDL_KMOD_ALT) != SDL.SDL_Keymod.SDL_KMOD_NONE;

            if (isshift)
            {
                sb.Append("Shift ");
            }

            if (isctrl)
            {
                sb.Append("Ctrl ");
            }

            if (isalt)
            {
                sb.Append("Alt ");
            }
        }
    }
}