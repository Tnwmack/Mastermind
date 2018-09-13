using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mastermind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind.Tests
{
    [TestClass()]
    public class GeneticSolverTests
    {
        [TestMethod()]
        public void CompareRowsTest()
        {
			GeneticSolver.GeneticSolverSettings Settings = new GeneticSolver.GeneticSolverSettings();
			GeneticSolver GS = new GeneticSolver();
            PrivateObject GSpriv = new PrivateObject(GS);

            RowState Answer = new RowState(0, 1, 2, 3);
            BoardRow Guess1 = new BoardRow(new RowState(1, 1, 1, 1), new RowScore(1, 0));

            RowState CheckRow1 = new RowState(0, 1, 2, 3);
            Assert.AreEqual(0, (int)GSpriv.Invoke("CompareRows", CheckRow1, Guess1));

			RowState CheckRow2 = new RowState(1, 1, 2, 2);
			Assert.AreEqual(-Settings.MatchScore, (int)GSpriv.Invoke("CompareRows", CheckRow2, Guess1));

			BoardRow Guess2 = new BoardRow(new RowState(3, 2, 1, 1), new RowScore(0, 3));

			Assert.AreEqual(0, (int)GSpriv.Invoke("CompareRows", CheckRow1, Guess2));
			Assert.AreEqual(0, (int)GSpriv.Invoke("CompareRows", CheckRow2, Guess2));

			RowState CheckRow3 = new RowState(1, 1, 2, 3);
			Assert.AreEqual(-Settings.PartialMatchScore, (int)GSpriv.Invoke("CompareRows", CheckRow3, Guess2));
		}
    }
}