﻿using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Section
{
    public int SectionId { get; set; }

    public string Name { get; set; } = null!;

    public int StadiumId { get; set; }

    public int? Capacity { get; set; }

    public decimal? Price { get; set; }

    public virtual Stadium Stadium { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}