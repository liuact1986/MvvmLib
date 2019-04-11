﻿using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    public interface IDialogService
    {
        Task<string> DisplayActionSheetAsync(string title, string message, string destruction, params string[] buttons);
        Task DisplayAlertAsync(string title, string message, string cancel);
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
    }
}