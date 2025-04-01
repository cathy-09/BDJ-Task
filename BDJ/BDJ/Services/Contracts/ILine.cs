﻿using BDJ.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BDJ.Services.Contracts
{
    public interface ILine
    {
        public Line Create(Line line);
        public List<Line> GetAll();
        public Line GetById(Guid id);
        public Line Delete(Line line);
        public Line Update(Line line);
    }
}
