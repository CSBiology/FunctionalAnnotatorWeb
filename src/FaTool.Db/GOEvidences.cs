using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaTool.Db
{

    public static class GOEvidences
    {

        private static GOEvidencesCollection values = null;

        private static void InitValues()
        {
            if (values == null)
            {
                var goEvidenceCodes = new GOEvidence[] { 

                    new GOEvidence("NR","Not Recorded"),
                    new GOEvidence("EXP","Inferred from Experiment"),
                    new GOEvidence("IDA","Inferred from Direct Assay"),
                    new GOEvidence("IPI","Inferred from Physical Interaction"),
                    new GOEvidence("IMP","Inferred from Mutant Phenotype"),
                    new GOEvidence("IGI","Inferred from Genetic Interaction"),
                    new GOEvidence("IEP","Inferred from Expression Pattern"),
                    new GOEvidence("ISS","Inferred from Sequence or Structural Similarity"),
                    new GOEvidence("ISO","Inferred from Sequence Orthology"),
                    new GOEvidence("ISA","Inferred from Sequence Alignment"),
                    new GOEvidence("ISM","Inferred from Sequence Model"),
                    new GOEvidence("IGC","Inferred from Genomic Context"),
                    new GOEvidence("IBA","Inferred from Biological aspect of Ancestor"),
                    new GOEvidence("IBD","Inferred from Biological aspect of Descendant"),
                    new GOEvidence("IKR","Inferred from Key Residues"),
                    new GOEvidence("IRD","Inferred from Rapid Divergence"),
                    new GOEvidence("RCA","Inferred from Reviewed Computational Analysis"),
                    new GOEvidence("TAS","Traceable Author Statement"),
                    new GOEvidence("NAS","Non-traceable Author Statement"),
                    new GOEvidence("IC","Inferred by Curator"),
                    new GOEvidence("ND","No biological Data available"),
                    new GOEvidence("IEA","Inferred from Electronic Annotation"),

                };

                values = new GOEvidencesCollection(goEvidenceCodes);
            }
        }

        public static GOEvidence Default { get { return Values.First(); } }

        public static IEnumerable<GOEvidence> Values
        {
            get
            {
                InitValues();
                return values;
            }
        }

        public static GOEvidence Resolve(string code)
        {
            InitValues();

            if (values.Contains(code))
                return values[code];
            else
                throw new KeyNotFoundException(code);
        }

        public static string ResolveName(string code)
        {
            return Resolve(code).Name;
        }

        public static bool CanResolve(string code)
        {
            InitValues();
            return values.Contains(code);
        }

        private class GOEvidencesCollection : KeyedCollection<string, GOEvidence>
        {

            public GOEvidencesCollection(IEnumerable<GOEvidence> values)
                : base(StringComparer.InvariantCultureIgnoreCase)
            {
                foreach (var item in values)
                    Add(item);
            }

            protected override string GetKeyForItem(GOEvidence item)
            {
                return item.Code;
            }
        }
    }

    public sealed class GOEvidence
    {
        private readonly string code = "NR";
        private readonly string name = "Not Recorded";

        internal GOEvidence(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("code");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            this.code = code;
            this.name = name;
        }

        public string Code { get { return code; } }
        public string Name { get { return name; } }
    }
}