(:action drop
	:parameters  (ball1  rooma left)
    :precondition  
		(and  
			(ball ball1) 
			(room rooma) 
			(gripper left)
			(carry ball1 left) 
			(at-robby rooma)
		)
    :effect 
		(and 
			(at ball1 rooma)
			(free left)
			(not (carry ball1 left))
		)
	)