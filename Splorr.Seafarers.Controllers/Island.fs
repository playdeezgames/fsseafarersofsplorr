namespace Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

module Island =
    let Create() : Island =
        {
            Name = ""
        }

    let SetName (name:string) (island:Island) : Island =
        {island with Name = name}

