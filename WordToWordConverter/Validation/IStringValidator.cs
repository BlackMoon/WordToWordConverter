
namespace WordToWordConverter.Validation
{
    /// <summary>
    /// Интерфейс строкового валидатора
    /// </summary>
    interface IStringValidator
    {
        string Value { get; set; }
        bool Validate(out string msg);
    }
}