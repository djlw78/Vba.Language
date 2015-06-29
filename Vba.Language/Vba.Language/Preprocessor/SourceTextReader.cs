﻿using System.IO;

namespace Vba.Language.Preprocessor
{
    public class SourceTextReader : ReadLineTextReader
    {
        private TextReader reader;
        private PreprocessorStateManager manager;
        private VbaHeader header;

        public SourceTextReader(string source) : this(new StringReader(source)) { }

        public SourceTextReader(TextReader reader)
        {
            this.reader = reader;
            manager = new PreprocessorStateManager();
            header = new VbaHeader();
        }

        public override string ReadLine()
        {
            // when ReadLine returns null we are at the end of the file.
            var line = reader.ReadLine();

            // first parse the header.
            while (line != null && header.ProcessHeaderStatement(line))
            {
                line = reader.ReadLine();
            }

            // after we exit the top loop, line will be the first non-header line.
            while (line != null) 
            {
                if (PreprocessorStateManager.IsPreprocessorStatement(line))
                {
                    manager.ExecutePreprocessorStatement(line);
                }
                else if (manager.IsActiveRegion)
                {
                    return line;
                }

                line = reader.ReadLine();
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}