(:action drop
	:parameters  (ball2  rooma left)
    :precondition  
		(and  
			(ball ball2) 
			(room rooma) 
			(gripper left)
			(carry ball2 left) 
			(at-robby rooma)
		)
    :effect 
		(and 
			(at ball2 rooma)
			(free left)
			(not (carry ball2 left))
		)
	)