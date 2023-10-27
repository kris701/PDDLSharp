(:action move
    :parameters (roomb rooma)
    :precondition 
        (and  
            (room roomb) 
            (room rooma) 
            (at-robby roomb)
        )
    :effect 
        (and
            (at-robby rooma)
		    (not (at-robby roomb))
        )
)