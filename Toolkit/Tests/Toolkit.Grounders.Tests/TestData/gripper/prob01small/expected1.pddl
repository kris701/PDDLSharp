(:action move
    :parameters (rooma rooma)
    :precondition 
        (and  
            (room rooma) 
            (room rooma) 
            (at-robby rooma)
        )
    :effect 
        (and
            (at-robby rooma)
		    (not (at-robby rooma))
        )
)