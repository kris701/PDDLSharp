(:action drop
	:parameters  (ball1  roomb right)
    :precondition  
		(and  
			(ball ball1) 
			(room roomb) 
			(gripper right)
			(carry ball1 right) 
			(at-robby roomb)
		)
    :effect 
		(and 
			(at ball1 roomb)
			(free right)
			(not (carry ball1 right))
		)
	)