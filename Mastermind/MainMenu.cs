using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mastermind
{
	/// <summary>
	/// The main form of the program
	/// </summary>
	public partial class MainMenu : Form
	{
		private BoardSettings Settings = new BoardSettings();
		private Solver AI;

		private Game Master;

		private ColorMapper Colors = new ColorMapper();

		private volatile bool GuessThreadWorking = false;

		/// <summary>
		/// Creates the main form of the program
		/// </summary>
		public MainMenu()
		{
			InitializeComponent();

			AITypeComboBox.SelectedIndex = 0;

			GameBoardControl.Colors = Colors;
			AnswerKeyControl.Colors = Colors;

			AnswerKeyControl.SetAnswer(new RowState(new byte[Settings.Columns]));
			AnswerKeyControl.NumColors = Settings.Colors;
		}

		private void GenerateGame(RowState Answer)
		{
			AI.Reset();

		GameBoard Board = new GameBoard(Settings.Colors, Settings.Columns, Settings.Rows, Answer);
			Master = new Game(Board, AI);

			GameBoardControl.ResetBoard();
			GameBoardControl.Columns = Settings.Columns;

			UpdateGameState();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void AITypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch(AITypeComboBox.SelectedIndex)
			{
				case 0:
					AI = new KnuthSolver();
					break;

				case 1:
					AI = new GeneticSolver();
					break;

				case 2:
					AI = new HybridSolver();
					break;

				case 3:
					AI = new RandomSolver();
					break;
			}

			GenerateGame(AnswerKeyControl.CurrentAnswer);
		}

		private void SolverButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			Master.AI.ShowSettingsDialog();
		}

		private void GetGuess()
		{
			GuessThreadWorking = true;

			RowState? RS = Master.GenerateGuess();

			this.Invoke((Action) delegate { 
				this.Cursor = Cursors.Default;

				if (RS != null)
				{
					GameBoardControl.AddRow(Master.Board.Guesses.Last());
				}

				UpdateGameState();
			});

			GuessThreadWorking = false;
		}

		private void GuessButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			this.Cursor = Cursors.WaitCursor;

		System.Threading.Thread GuessThread = new System.Threading.Thread(new System.Threading.ThreadStart(GetGuess));

			GuessThread.Start();
		}

		private void UpdateGameState()
		{
			switch(Master.Board.CurrentGameState)
			{
				case GameBoard.GameState.InProgress:
					GameStateLabel.Text = "Game In Progress";
					break;

				case GameBoard.GameState.Won:
					GameStateLabel.Text = "Game Won!";
					break;

				case GameBoard.GameState.Lost:
					GameStateLabel.Text = "Game Lost!";
					break;
			}

			SolverStatusLabel.Text = AI.GetMessage();
		}

		private void ResetButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			GenerateGame(AnswerKeyControl.CurrentAnswer);
		}

		private void BoardSettingsButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

		BoardSettingsForm SettForm = new BoardSettingsForm(Settings);

			if(SettForm.ShowDialog() == DialogResult.OK)
			{
				Settings = SettForm.Settings;

				AnswerKeyControl.SetAnswer(new RowState(new byte[Settings.Columns]));
				AnswerKeyControl.NumColors = Settings.Colors;

				GenerateGame(AnswerKeyControl.CurrentAnswer);
			}
		}

		private void AnswerKeyControl_OnAnswerChanged(RowState NewAnswer)
		{
			if (GuessThreadWorking)
			{
				AnswerKeyControl.SetAnswer(Master.Board.Answer);
				return;
			}

			GenerateGame(NewAnswer);
		}

		private void TestButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			TestForm TestingForm = new TestForm();
			//Events called in seprate thread
			TestingForm.OnRunClicked += TestingForm_OnRunClicked;
			TestingForm.OnStopClicked += TestingForm_OnCancelClicked;

			TestingForm.ShowDialog();
		}

		private volatile bool CancelTesting;

		private void TestingForm_OnCancelClicked(TestForm Sender)
		{
			CancelTesting = true;
		}

		private void TestingForm_OnRunClicked(TestForm Sender, int Iterations)
		{
			CancelTesting = false;
			Random Rand = new Random();
			System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();

			List<int> NumGuesses = new List<int>();
			List<double> ProcessTime = new List<double>();
			int Failed = 0;

			for (int i = 0; i < Iterations && !CancelTesting; i ++)
			{
				RowState Answer = RowState.GetRandomColors(Rand, Settings.Colors, Settings.Columns);
				GameBoard Board = new GameBoard(Settings.Colors, Settings.Columns, Settings.Rows, Answer);
				Game TestGame = new Game(Board, AI);

				int GuessRow = 0;

				Timer.Restart();

				for (; GuessRow < Settings.Rows; GuessRow++)
				{
					TestGame.GenerateGuess();

					if (TestGame.Board.CurrentGameState != GameBoard.GameState.InProgress)
						break;
				}
				
				Timer.Stop();

				if (TestGame.Board.CurrentGameState == GameBoard.GameState.Lost)
				{
					Failed++;
				}

				NumGuesses.Add(GuessRow + 1);
				ProcessTime.Add(Timer.Elapsed.TotalSeconds);

				AI.Reset();

				Sender.UpdateResults(i + 1, ProcessTime.Average(), NumGuesses.Average(), Failed);
			}

			Sender.OnTestsCompleted();
		}
	}
}
