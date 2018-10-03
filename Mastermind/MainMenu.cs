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

		/// <summary>
		/// Resets the global game state and sets up a new game.
		/// </summary>
		/// <param name="Answer">The answer to use for the new game.</param>
		private void GenerateGame(RowState Answer)
		{
			AI.Reset();

		GameBoard Board = new GameBoard(Settings.Colors, Settings.Columns, Settings.Rows, Answer);
			Master = new Game(Board, AI);

			GameBoardControl.ResetBoard();
			GameBoardControl.Columns = Settings.Columns;

			UpdateGameState();
		}

		/// <see cref="Form.OnLoad(EventArgs)"/>
		private void MainMenu_Load(object sender, EventArgs e)
		{
			
		}

		/// <see cref="Form.OnShown(EventArgs)"/>
		private void MainMenu_Shown(object sender, EventArgs e)
		{
			
		}

		/// <see cref="ComboBox.OnSelectedIndexChanged(EventArgs)"/>
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

		/// <summary>
		/// Shows the currently selected solver's settings dialog.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void SolverButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			Master.AI.ShowSettingsDialog();
		}

		/// <summary>
		/// Gets a guess from the current AI system. Runs in a separate thread.
		/// </summary>
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

		/// <summary>
		/// Begins the process of getting a guess.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void GuessButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			this.Cursor = Cursors.WaitCursor;

		System.Threading.Thread GuessThread = new System.Threading.Thread(new System.Threading.ThreadStart(GetGuess));

			GuessThread.Start();
		}

		/// <summary>
		/// Updates the game state label.
		/// </summary>
		private void UpdateGameState()
		{
			switch(Master.Board.CurrentGameState)
			{
				case GameBoard.GameState.InProgress:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 8.0f, FontStyle.Regular, GraphicsUnit.Point);
					GameStateLabel.ForeColor = SystemColors.ControlText;
					GameStateLabel.Text = "Game In Progress";
					GameStateLabel.Invalidate();
					break;

				case GameBoard.GameState.Won:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold, GraphicsUnit.Point);
					GameStateLabel.ForeColor = Color.Green;
					GameStateLabel.Text = "Game Won!";
					break;

				case GameBoard.GameState.Lost:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold, GraphicsUnit.Point);
					GameStateLabel.ForeColor = Color.DarkRed;
					GameStateLabel.Text = "Game Lost!";
					break;
			}

			SolverStatusLabel.Text = AI.GetMessage();
		}

		/// <summary>
		/// Resets the game at the user's request.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void ResetButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			GenerateGame(AnswerKeyControl.CurrentAnswer);
		}

		/// <summary>
		/// Opens the game board settings dialog.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
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

		/// <summary>
		/// Called when the answer is changed by the user. Either rejects the 
		/// user input or generates a new game.
		/// </summary>
		/// <param name="NewAnswer">The answer the user selected.</param>
		private void AnswerKeyControl_OnAnswerChanged(RowState NewAnswer)
		{
			if (GuessThreadWorking)
			{
				AnswerKeyControl.SetAnswer(Master.Board.Answer);
				return;
			}

			GenerateGame(NewAnswer);
		}

		/// <summary>
		/// Opens the test run form.
		/// </summary>
		/// <param name="sender">The control handle.</param>
		/// <param name="e"><see cref="Control.OnClick(EventArgs)"/></param>
		private void TestButton_Click(object sender, EventArgs e)
		{
			if (GuessThreadWorking)
				return;

			TestForm TestingForm = new TestForm();
			//Events called in separate thread
			TestingForm.OnRunClicked += TestingForm_OnRunClicked;
			TestingForm.OnStopClicked += TestingForm_OnCancelClicked;

			TestingForm.ShowDialog();
		}

		private volatile bool CancelTesting;

		/// <summary>
		/// Called when the user wants to cancel the testing routine.
		/// </summary>
		/// <param name="Sender">The test form handle.</param>
		private void TestingForm_OnCancelClicked(TestForm Sender)
		{
			CancelTesting = true;
		}

		/// <summary>
		/// Begins a test run.
		/// </summary>
		/// <param name="Sender">The test form handle.</param>
		/// <param name="Iterations">The number of runs to perform.</param>
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
