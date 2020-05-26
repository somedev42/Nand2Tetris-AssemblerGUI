//testfile for the new Computer and Assembler


//sets the m register of the addr var to 50
@50
D=A
@addr
M=D


//loop to set the number 8 in every m-register from the register 50 to 81
(LOOP)
@8 		//can be changed
D=A 
@addr
A=M
M=D
@addr
M=M+1
@82
D=A
@addr
D=D-M
@LOOP
D;JGT

//set the d register to 4
@4	//can also be changed
D=A 
D=-D


//execute all possible operations with the new alu 
//and save the result in the ram
//every operation is with M=8 and D=-4
@50
M=M*D
@51
M=M/D
@52
M=M+D^2
@53
M=M-D^2
@54
M=M*D^2
@55
M=M/D^2
@56
M=-M+D
@57
M=-M-D
@58
M=-M*D
@59
M=-M/D
@60
M=-M+D^2
@61
M=-M-D^2
@62
M=-M*D^2
@63
M=-M/D^2
@64
M=M^2+D
@65
M=M^2-D
@66
M=M^2*D
@67
M=M^2/D
@68
M=M^2+D^2
@69
M=M^2-D^2
@70
M=M^2*D^2
@71
M=M^2/D^2
@72
M=-M^2+D
@73
M=-M^2-D
@74
M=-M^2*D
@75
M=-M^2/D
@76
M=-M^2+D^2
@77
M=-M^2-D^2
@78
M=-M^2*D^2
@79
M=-M^2/D^2
@80
M=M^2
@81
M=D^2

(END)
 @END
 0;JMP
