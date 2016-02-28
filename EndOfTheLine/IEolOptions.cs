using System;

namespace EndOfTheLine
{
    public interface IEolOptions
    {
        VisibilityPolicy Visibility { get; set; }
        EndingRepresentationStyle Style { get; set; }
        bool WhenCrlf { get; set; }
        bool WhenLf { get; set; }
        bool WhenOther { get; set; }

        event EventHandler OptionChanged;
    }
}