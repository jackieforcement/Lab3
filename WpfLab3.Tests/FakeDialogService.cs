using System;
using WpfLab3.Services;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests
{
    public class FakeDialogService : IDialogService
    {
        private Func<TaskEditViewModel, bool>? _editorHandler;
        private bool _confirmResult;

        public FakeDialogService()
        {
            _editorHandler = null;
            _confirmResult = true;
        }

        public Func<TaskEditViewModel, bool>? EditorHandler
        {
            get
            {
                return _editorHandler;
            }
            set
            {
                _editorHandler = value;
            }
        }

        public bool ConfirmResult
        {
            get
            {
                return _confirmResult;
            }
            set
            {
                _confirmResult = value;
            }
        }

        public bool ShowTaskEditor(TaskEditViewModel viewModel)
        {
            if (_editorHandler == null)
            {
                return false;
            }
            return _editorHandler(viewModel);
        }

        public bool Confirm(string message, string title = "Подтверждение")
        {
            return _confirmResult;
        }
    }
}
