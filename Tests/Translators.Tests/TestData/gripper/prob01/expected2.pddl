(:action move
    :parameters (rooma roomb)
    :precondition 
        (and  
            (room rooma) 
            (room roomb) 
            (at-robby rooma)
        )
    :effect 
        (and
            (at-robby roomb)
		    (not (at-robby rooma))
        )
)