using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WpfLab3.Mvvm
{
    public class ObservableValidator : ObservableObject, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors;

        public ObservableValidator()
        {
            _errors = new Dictionary<string, List<string>>();
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0;
            }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName == null)
            {
                List<string> all = new List<string>();
                foreach (KeyValuePair<string, List<string>> entry in _errors)
                {
                    foreach (string message in entry.Value)
                    {
                        all.Add(message);
                    }
                }
                return all;
            }
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            return new List<string>();
        }

        protected void ValidateProperty(object? value, string propertyName)
        {
            PropertyInfo? propertyInfo = GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return;
            }

            ValidationContext context = new ValidationContext(this);
            context.MemberName = propertyName;
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateProperty(value, context, results);

            List<string> messages = new List<string>();
            foreach (ValidationResult result in results)
            {
                if (result.ErrorMessage != null)
                {
                    messages.Add(result.ErrorMessage);
                }
            }

            bool changed = false;
            if (messages.Count == 0)
            {
                if (_errors.ContainsKey(propertyName))
                {
                    _errors.Remove(propertyName);
                    changed = true;
                }
            }
            else
            {
                _errors[propertyName] = messages;
                changed = true;
            }

            if (changed)
            {
                EventHandler<DataErrorsChangedEventArgs>? handler = ErrorsChanged;
                if (handler != null)
                {
                    handler(this, new DataErrorsChangedEventArgs(propertyName));
                }
                OnPropertyChanged("HasErrors");
            }
        }

        protected void ValidateAllProperties()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                object[] attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);
                if (attributes.Length > 0)
                {
                    object? value = property.GetValue(this);
                    ValidateProperty(value, property.Name);
                }
            }
        }
    }
}
