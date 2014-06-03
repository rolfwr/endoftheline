using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EndOfTheLine.Test
{
    public class EolAdornmentTest
    {
        [Fact]
        public void RefreshAfterNormalEdit()
        {
            var lineView = new AdornedTextViewStub();
            IReadOnlyList<ViewLineStub> changedLines =
                new List<ViewLineStub>(lineView.Lines.Skip(3).Take(5));
            foreach (var line in changedLines)
            {
                line.AdornmentCount = 0;
            }

            EolAdornment.RefreshAdornments(lineView, changedLines);

            AssertAllLinesHaveSingleAdornment(lineView);
        }

        [Fact]
        public void RefreshAfterPressingEnterOnEmptyLine()
        {
            // When pressing enter on an empty line VS2013 removes the
            // adornment of the empty line, but only sends the newly
            // created line below in e.NewOrReformattedLines
            var lineView = new AdornedTextViewStub();
            lineView.Lines[4].AdornmentCount = 0;
            var newLine = lineView.Lines[5];
            newLine.AdornmentCount = 0;
            var changedLines = new[] { newLine };

            EolAdornment.RefreshAdornments(lineView, changedLines);
            AssertAllLinesHaveSingleAdornment(lineView);
        }

        private static void AssertAllLinesHaveSingleAdornment(IAdornedTextView<ViewLineStub> adornedTextView)
        {
            Action<ViewLineStub> lineMustHaveOneAdornment =
                line => Assert.Equal(1, line.AdornmentCount);
            Assert.Collection(adornedTextView.Lines,
                Enumerable.Repeat(lineMustHaveOneAdornment, 10).ToArray());
        }

        private class AdornedTextViewStub : IAdornedTextView<ViewLineStub>
        {
            public AdornedTextViewStub()
            {
                Lines = new List<ViewLineStub>();
                for (var i = 0; i < 10; ++i)
                {
                    Lines.Add(new ViewLineStub { AdornmentCount = 1 });
                }
            }

            public void AddAdornmentToLine(ViewLineStub line)
            {
                ++line.AdornmentCount;
            }

            public IList<ViewLineStub> Lines { get; private set; }
            public void ClearAdornmentsFromLine(ViewLineStub line)
            {
                line.AdornmentCount = 0;
            }
        }

        private class ViewLineStub
        {
            public int AdornmentCount;
        }
    }
}