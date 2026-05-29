using NUnit.Framework;
using WpfLab3.Models;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests
{
    [TestFixture]
    public class TaskEditViewModelTests
    {
        [Test]
        public void NewEditor_WithoutTitle_HasValidationErrorsAndSaveDisabled()
        {
            TaskEditViewModel vm = new TaskEditViewModel();

            Assert.That(vm.HasErrors, Is.True);
            Assert.That(vm.SaveCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void Editor_TitleTooLong_ReportsValidationError()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            vm.Title = new string('x', 101);

            Assert.That(vm.HasErrors, Is.True);
            Assert.That(vm.SaveCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void Editor_DescriptionTooLong_ReportsValidationError()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            vm.Title = "ok";
            vm.Description = new string('x', 501);

            Assert.That(vm.HasErrors, Is.True);
        }

        [Test]
        public void Editor_ValidInput_AllowsSave()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            vm.Title = "Buy milk";
            vm.Description = "2% low fat";

            Assert.That(vm.HasErrors, Is.False);
            Assert.That(vm.SaveCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void Editor_FromExistingTask_PrefillsFields()
        {
            TodoTask source = new TodoTask();
            source.Title = "Existing";
            source.Description = "desc";
            source.IsCompleted = true;

            TaskEditViewModel vm = new TaskEditViewModel(source);

            Assert.That(vm.Title, Is.EqualTo("Existing"));
            Assert.That(vm.Description, Is.EqualTo("desc"));
            Assert.That(vm.IsCompleted, Is.True);
        }

        [Test]
        public void ToModel_AppliesTrimmedValuesAndNormalizesEmptyDescription()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            vm.Title = "  Hello  ";
            vm.Description = "   ";
            vm.IsCompleted = true;

            TodoTask model = vm.ToModel();

            Assert.That(model.Title, Is.EqualTo("Hello"));
            Assert.That(model.Description, Is.Null);
            Assert.That(model.IsCompleted, Is.True);
        }

        [Test]
        public void Save_WhenValid_RaisesRequestCloseWithTrue()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            vm.Title = "ok";
            bool? captured = null;
            void Handler(object? sender, bool? result)
            {
                captured = result;
            }
            vm.RequestClose += Handler;

            vm.SaveCommand.Execute(null);

            Assert.That(captured, Is.True);
            Assert.That(vm.DialogResult, Is.True);
        }

        [Test]
        public void Cancel_RaisesRequestCloseWithFalse()
        {
            TaskEditViewModel vm = new TaskEditViewModel();
            bool? captured = null;
            void Handler(object? sender, bool? result)
            {
                captured = result;
            }
            vm.RequestClose += Handler;

            vm.CancelCommand.Execute(null);

            Assert.That(captured, Is.False);
            Assert.That(vm.DialogResult, Is.False);
        }
    }
}
