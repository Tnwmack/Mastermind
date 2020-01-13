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
		private readonly Game Master;
		private readonly ColorMapper Colors = new ColorMapper();

		private volatile bool GuessThreadWorking = false;

		private readonly Timer CloseTimer = new Timer();

		/// <summary>
		/// Creates the main form of the program
		/// </summary>
		public MainMenu()
		{
			InitializeComponent();
			
			components = new Container();
			components.Add(CloseTimer); //This is so CloseTimer will be disposed on form close in the designer created Dispose method

			GameBoardControl.Colors = Colors;
			AnswerKeyControl.Colors = Colors;

			Master = new Game();
			Master.OnAIStatusChange += OnSetMessage;

			AnswerKeyControl.NumColors = Master.Settings.Colors;
			AnswerKeyControl.SetAnswer(new RowState(new byte[Master.Settings.Columns]));

			AITypeComboBox.SelectedIndex = 0;
		}

		/// <summary>
		/// Resets the global game state and sets up a new game.
		/// </summary>
		private void GenerateGame()
		{
			Master.GenerateBoard(AnswerKeyControl.CurrentAnswer);
			Master.Board.OnBoardChanged += OnBoardChanged;
			Master.Board.OnGameStateChanged += OnGameStateChanged;

			GameBoardControl.ResetBoard();
			GameBoardControl.Columns = Master.Settings.Columns;

			UpdateGameState();
		}

		private void OnBoardChanged(GameBoard Sender)
		{
			if (Sender.Guesses.Count > 0)
				GameBoardControl.AddRow(Sender.Guesses.Last());
			else
				GameBoardControl.ResetBoard();
		}

		private void OnGameStateChanged(GameBoard Sender)
		{
			UpdateGameState();
		}

		/// <summary>
		/// Updates the game state label.
		/// </summary>
		private void UpdateGameState()
		{
			if (GameStateLabel.InvokeRequired)
			{
				GameStateLabel.Invoke(new Action(UpdateGameState));
				return;
			}

			switch (Master.Board.CurrentGameState)
			{
				case GameBoard.GameState.InProgress:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 8.0f, FontStyle.Regular, GraphicsUnit.Point);
					GameStateLabel.ForeColor = SystemColors.ControlText;
					GameStateLabel.Text = Properties.Resources.GameStateLabel_InProgress;
					break;

				case GameBoard.GameState.Won:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold, GraphicsUnit.Point);
					GameStateLabel.ForeColor = Color.Green;
					GameStateLabel.Text = Properties.Resources.GameStateLabel_Won;
					break;

				case GameBoard.GameState.Lost:
					GameStateLabel.Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold, GraphicsUnit.Point);
					GameStateLabel.ForeColor = Color.DarkRed;
					GameStateLabel.Text = Properties.Resources.GameStateLabel_Lost;
					break;
			}
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
					Master.SetAI(Game.SolverType.Knuth);
					break;

				case 1:
					Master.SetAI(Game.SolverType.Genetic);
					break;

				case 2:
					Master.SetAI(Game.SolverType.Hybrid);
					break;

				case 3:
					Master.SetAI(Game.SolverType.Random);
					break;

				default:
					return;
			}
		}

		/// <summary>
		/// Callback event when the solver wants to display a message.
		/// </summary>
		/// <param name="Message">Message to display.</param>
		private void OnSetMessage(string Message)
		{
			SolverStatusLabel.Text = Message;
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

			Master.ShowAISettingsDialog();
		}

		/// <summary>
		/// Gets a guess from the current AI system. Runs in a separate thread.
		/// </summary>
		private void GetGuess()
		{
			this.Invoke((Action)delegate {
				this.Cursor = Cursors.WaitCursor;
			});

			GuessThreadWorking = true;

			Master.AddGuessFromAI();

			GuessThreadWorking = false;

			this.Invoke((Action)delegate {
				this.Cursor = Cursors.Default;
			});
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

		System.Threading.Thread GuessThread = new System.Threading.Thread(new System.Threading.ThreadStart(GetGuess));

			GuessThread.Start();
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

			Master.Reset();
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

			using (BoardSettingsForm SettForm = new BoardSettingsForm(Master.Settings))
			{
				if (SettForm.ShowDialog() == DialogResult.OK)
				{
					Master.SetBoardSettings((BoardSettings)SettForm.Settings.Clone());

					AnswerKeyControl.SetAnswer(new RowState(new byte[Master.Settings.Columns]));
					AnswerKeyControl.NumColors = Master.Settings.Colors;

					GenerateGame();
				}
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
				AnswerKeyControl.SetAnswer(Master.Board.Answer); //Reset answer
				return;
			}

			GenerateGame();
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

			using (TestForm TestingForm = new TestForm())
			{
				TestingForm.OnRunClicked += TestingForm_OnRunClicked;
				TestingForm.OnStopClicked += TestingForm_OnCancelClicked;

				TestingForm.ShowDialog();
			}
		}

		private volatile bool CancelTesting;

		/// <summary>
		/// Called when the user wants to cancel the testing routine.
		/// </summary>
		/// <param name="Sender">The test form handle.</param>
		private void TestingForm_OnCancelClicked(TestForm Sender)
		{
			Master?.AI.Abort();
			CancelTesting = true;
		}

		/// <summary>
		/// Begins a test run.
		/// </summary>
		/// <param name="Sender">The test form handle.</param>
		/// <param name="Iterations">The number of runs to perform.</param>
		private async void TestingForm_OnRunClicked(TestForm Sender, int Iterations)
		{
			CancelTesting = false;
			Random Rand = new Random();
			System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();

			List<int> NumGuesses = new List<int>();
			List<double> ProcessTime = new List<double>();
			int Failed = 0;

			for (int i = 0; i < Iterations && !CancelTesting; i ++)
			{
				RowState Answer = RowState.GetRandomColors(Rand, Master.Settings.Colors, Master.Settings.Columns);
				AnswerKeyControl.SetAnswer(Answer);

				GenerateGame();

				int GuessRow = 0;

				Timer.Restart();

				while (Master.Board.CurrentGameState == GameBoard.GameState.InProgress)
				{
					await Task.Run(new Action(GetGuess)).ConfigureAwait(true);
					GuessRow++;
				}
				
				Timer.Stop();

				if (Master.Board.CurrentGameState == GameBoard.GameState.Lost)
				{
					Failed++;
				}
				else
				{
					NumGuesses.Add(GuessRow);
				}

				ProcessTime.Add(Timer.Elapsed.TotalSeconds);
				Sender.UpdateResults(i + 1, ProcessTime.Average(), NumGuesses.Count > 0 ? NumGuesses.Average() : 0, Failed);
			}

			Sender.OnTestsCompleted();
		}

		private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (GuessThreadWorking)
			{
				e.Cancel = true;

				if (!CloseTimer.Enabled)
				{
					GameStateLabel.Text = Properties.Resources.GameStateLabel_Aborting;
					Master.AI.Abort();

					CloseTimer.Interval = 1000;

					CloseTimer.Tick += new EventHandler((s, evt) =>
					{
						if (!GuessThreadWorking)
						{
							CloseTimer.Stop();
							this.Close();
						}
					});

					CloseTimer.Start();
				}
			}
		}
	}
}
