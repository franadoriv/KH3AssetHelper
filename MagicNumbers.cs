﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UEKH3
{
    public class MagicNumbers
    {
        public  int[] bpp = new int[116]
{
      0,
      128,
      128,
      128,
      128,
      96,
      96,
      96,
      96,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      64,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      16,
      8,
      8,
      8,
      8,
      8,
      8,
      1,
      32,
      32,
      32,
      4,
      4,
      4,
      8,
      8,
      8,
      8,
      8,
      8,
      4,
      4,
      4,
      8,
      8,
      8,
      16,
      16,
      32,
      32,
      32,
      32,
      32,
      32,
      32,
      8,
      8,
      8,
      8,
      8,
      8,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      16
};
        public  int[] pixbl = new int[278];

        public MagicNumbers()
        {
            for (int index = 0; index < 278; ++index)
                pixbl[index] = 1;

            for (int index = 70; index <= 84; ++index)
                pixbl[index] = 4;

            for (int index = 94; index <= 99; ++index)
                pixbl[index] = 4;
        }
    }
}
