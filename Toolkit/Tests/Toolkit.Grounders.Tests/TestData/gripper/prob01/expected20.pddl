(:action drop
	:parameters  (ball2  roomb right)
    :precondition  
		(and  
			(ball ball2) 
			(room roomb) 
			(gripper right)
			(carry ball2 right) 
			(at-robby roomb)
		)
    :effect 
		(and 
			(at ball2 roomb)
			(free right)
			(not (carry ball2 right))
		)
	)