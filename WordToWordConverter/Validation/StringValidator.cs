using System.Text.RegularExpressions;

namespace WordToWordConverter.Validation
{
    /// <summary>
    /// Валидатор строк
    /// </summary>
    public class StringValidator : IStringValidator
    {
        public string Value { get; set; }

        public bool Validate(out string msg)
        {
            msg = "Некорректный символ!";
            return new Regex("^[a-zA-ZА-Яа-я]+$").IsMatch(Value);
        }
    }
}
