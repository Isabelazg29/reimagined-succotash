﻿using System;
using System.Collections.Generic;

namespace Glamping_Addventure2.Models;

public partial class DetalleReservaPaquete
{
    public int IddetalleReservaPaquetes { get; set; }

    public int? Idreserva { get; set; }

    public int? Cantidad { get; set; }

    public int? Precio { get; set; }

    public string? Estado { get; set; }

    public int? Idpaquete { get; set; }

    public virtual Paquete? IdpaqueteNavigation { get; set; }

    public virtual Reserva? IdreservaNavigation { get; set; }
}
