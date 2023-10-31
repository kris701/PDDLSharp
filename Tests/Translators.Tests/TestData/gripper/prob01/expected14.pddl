(:action drop
	:parameters  (ball1  rooma right)
    :precondition  
		(and  
			(ball ball1) 
			(room rooma) 
			(gripper right)
			(carry ball1 right) 
			(at-robby rooma)
		)
    :effect 
		(and 
			(at ball1 rooma)
			(free right)
			(not (carry ball1 right))
		)
	)