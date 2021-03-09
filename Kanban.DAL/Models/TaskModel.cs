﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kanban.DAL.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public State? State { get; set; }
    }
}