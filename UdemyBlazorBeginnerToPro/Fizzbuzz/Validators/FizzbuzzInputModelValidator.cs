using Fizzbuzz.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Fizzbuzz.Validators
{
    public sealed class FizzbuzzInputModelValidator : ComponentBase
    {
        private ValidationMessageStore _store = null!;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (CurrentEditContext is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FizzbuzzInputModelValidator)} requires a cascading parameter of type {nameof(EditContext)}."
                );
            }

            _store = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnFieldChanged += (_, e) => ValidateField(e.FieldIdentifier);
            CurrentEditContext.OnValidationRequested += (_, e) => ValidateAllFields();
        }

        private void ValidateAllFields()
        {
            var model =
                CurrentEditContext.Model as FizzbuzzInputModel
                ?? throw new InvalidOperationException(
                    $"{nameof(FizzbuzzInputModelValidator)} requires a mode of type {nameof(FizzbuzzInputModel)}."
                );

            _store.Clear();

            ValidateField(new FieldIdentifier(model, nameof(FizzbuzzInputModel.FizzNumber)));
            ValidateField(new FieldIdentifier(model, nameof(FizzbuzzInputModel.BuzzNumber)));
            ValidateField(new FieldIdentifier(model, nameof(FizzbuzzInputModel.MinInclusive)));
            ValidateField(new FieldIdentifier(model, nameof(FizzbuzzInputModel.MaxInclusive)));

            CurrentEditContext.NotifyValidationStateChanged();
        }

        private void ValidateField(FieldIdentifier fieldIdentifier)
        {
            var model =
                CurrentEditContext.Model as FizzbuzzInputModel
                ?? throw new InvalidOperationException(
                    $"{nameof(FizzbuzzInputModelValidator)} requires a mode of type {nameof(FizzbuzzInputModel)}."
                );

            _store.Clear(fieldIdentifier);

            if (fieldIdentifier.FieldName == nameof(FizzbuzzInputModel.FizzNumber))
            {
                if (model.FizzNumber >= model.BuzzNumber)
                {
                    _store.Add(fieldIdentifier, "Fizz value must be less than Buzz value.");
                }
            }

            if (fieldIdentifier.FieldName == nameof(FizzbuzzInputModel.BuzzNumber))
            {
                if (model.BuzzNumber <= model.FizzNumber)
                {
                    _store.Add(fieldIdentifier, "Buzz value must be greater than Fizz value.");
                }
            }

            if (fieldIdentifier.FieldName == nameof(FizzbuzzInputModel.MaxInclusive))
            {
                var minStopValue = model.FizzNumber * model.BuzzNumber;

                if (model.MaxInclusive < minStopValue)
                {
                    _store.Add(
                        fieldIdentifier,
                        $"Stop value must be greater than or equal to the product of Fizz and Buzz values ({minStopValue})."
                    );
                }
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
