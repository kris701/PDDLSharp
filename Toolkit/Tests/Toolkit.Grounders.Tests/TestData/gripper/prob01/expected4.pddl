(:action move
    :parameters (roomb roomb)
    :precondition 
        (and  
            (room roomb) 
            (room roomb) 
            (at-robby roomb)
        )
    :effect 
        (and
            (at-robby roomb)
		    (not (at-robby roomb))
        )
)