(:action pick
	:parameters (ball2 roomb right)
	:precondition  
		(and  
			(ball ball2) 
			(room roomb) 
			(gripper right)
			(at ball2 roomb) 
			(at-robby roomb) 
			(free right)
		)
	:effect 
		(and 
			(carry ball2 right)
		    (not (at ball2 roomb)) 
		    (not (free right))
		)
	)