using NUnit.Framework;
using WpfLab3.Models;
using WpfLab3.ViewModels;

namespace WpfLab3.Tests;

[TestFixture]
public class TaskEditViewModelTests
{
    [Test]
    public void NewEditor_WithoutTitle_HasValidationErrorsAndSaveDisabled()
    {
        var vm = new TaskEditViewModel();

        Assert.That(vm.HasErrors, Is.True);
        Assert.That(vm.SaveCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void Editor_TitleTooLong_ReportsValidationError()
    {
        var vm = new TaskEditViewModel { Title = new string('x', 101) };

        Assert.That(vm.HasErrors, Is.True);
        Assert.That(vm.SaveCommand.CanExecute(null), Is.False);
    }

    [Test]
    public void Editor_DescriptionTooLong_ReportsValidationError()
    {
        var vm = new TaskEditViewModel
        {
            Title = "ok",
            Description = new string('x', 501)
        };

        Assert.That(vm.HasErrors, Is.True);
    }

    [Test]
    public void Editor_ValidInput_AllowsSave()
    {
        var vm = new TaskEditViewModel
        {
            Title = "Buy milk",
            Description = "2% low fat"
        };

        Assert.That(vm.HasErrors, Is.False);
        Assert.That(vm.SaveCommand.CanExecute(null), Is.True);
    }

    [Test]
    public void Editor_FromExistingTask_PrefillsFields()
    {
        var source = new TodoTask
        {
            Title = "Existing",
            Description = "desc",
            IsCompleted = true
        };

        var vm = new TaskEditViewModel(source);

        Assert.That(vm.Title, Is.EqualTo("Existing"));
        Assert.That(vm.Description, Is.EqualTo("desc"));
        Assert.That(vm.IsCompleted, Is.True);
    }

    [Test]
    public void ToModel_AppliesTrimmedValuesAndNormalizesEmptyDescription()
    {
        var vm = new TaskEditViewModel
        {
            Title = "  Hello  ",
            Description = "   ",
            IsCompleted = true
        };

        var model = vm.ToModel();

        Assert.That(model.Title, Is.EqualTo("Hello"));
        Assert.That(model.Description, Is.Null);
        Assert.That(model.IsCompleted, Is.True);
    }

    [Test]
    public void Save_WhenValid_RaisesRequestCloseWithTrue()
    {
        var vm = new TaskEditViewModel { Title = "ok" };
        bool? captured = null;
        vm.RequestClose += (_, r) => captured = r;

        vm.SaveCommand.Execute(null);

        Assert.That(captured, Is.True);
        Assert.That(vm.DialogResult, Is.True);
    }

    [Test]
    public void Cancel_RaisesRequestCloseWithFalse()
    {
        var vm = new TaskEditViewModel();
        bool? captured = null;
        vm.RequestClose += (_, r) => captured = r;

        vm.CancelCommand.Execute(null);

        Assert.That(captured, Is.False);
        Assert.That(vm.DialogResult, Is.False);
    }
}
