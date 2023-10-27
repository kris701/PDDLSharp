(:action drop
	:parameters  (ball2  rooma right)
    :precondition  
		(and  
			(ball ball2) 
			(room rooma) 
			(gripper right)
			(carry ball2 right) 
			(at-robby rooma)
		)
    :effect 
		(and 
			(at ball2 rooma)
			(free right)
			(not (carry ball2 right))
		)
	)