using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mastermind
{
	class ColorMapper : IColorMapping
	{
		private Brush[] Brushes = new Brush[20];

		public ColorMapper()
		{
			Brushes[0] = new SolidBrush(Color.LightPink);
			Brushes[1] = new SolidBrush(Color.Fuchsia);
			Brushes[2] = new SolidBrush(Color.Red);
			Brushes[3] = new SolidBrush(Color.SaddleBrown);

			Brushes[4] = new SolidBrush(Color.LemonChiffon);
			Brushes[5] = new SolidBrush(Color.Yellow);
			Brushes[6] = new SolidBrush(Color.Orange);
			
			Brushes[7] = new SolidBrush(Color.LightGreen);
			Brushes[8] = new SolidBrush(Color.DarkSeaGreen);
			Brushes[9] = new SolidBrush(Color.DarkGreen);
			
			Brushes[10] = new SolidBrush(Color.LightBlue);
			Brushes[11] = new SolidBrush(Color.CadetBlue);
			Brushes[12] = new SolidBrush(Color.DarkBlue);
			
			Brushes[13] = new SolidBrush(Color.Indigo);
			Brushes[14] = new SolidBrush(Color.DarkViolet);
			Brushes[15] = new SolidBrush(Color.Violet);
			
			Brushes[16] = new SolidBrush(Color.White);
			Brushes[17] = new SolidBrush(Color.LightGray);
			Brushes[18] = new SolidBrush(Color.DarkGray);
			Brushes[19] = new SolidBrush(Color.Black);
		}

		public Brush GetBrush(int ColorID)
		{
			if (ColorID >= 0 && ColorID < Brushes.Length)
				return Brushes[ColorID];

			return Brushes[Brushes.Length - 1];
		}
	}
}
