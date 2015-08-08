using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RDotNet;

namespace Clustering
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static REngine engine;
        static void Main()
        {
            string rhome = System.Environment.GetEnvironmentVariable("R_HOME");
            if (string.IsNullOrEmpty(rhome))
                rhome = @"C:\Program Files\R\R-3.1.1";

            System.Environment.SetEnvironmentVariable("R_HOME", rhome);
            System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + rhome + @"\bin\i386");

            REngine.SetEnvironmentVariables();
            // There are several options to initialize the engine, but by default the following suffice:
            engine = REngine.GetInstance();

            engine.Initialize();
            FA();
            KMeansR();
            PSOtoR();
            engine.Dispose();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void FA()
        {
            //RGraphAppHook cbt;
            engine.Evaluate("library(psych)");
            engine.Evaluate("z <- attitude");
            engine.Evaluate("y <- 2");

            // .NET Framework array to R vector.
            //cbt = new RGraphAppHook { GraphControl = Form1.};
            Console.Write(printFromR("fa.parallel(attitude)"));
            Console.Write(printFromR("KMO(cor(attitude))"));
            Console.Write(printFromR("KMO(cov(attitude))"));

            engine.Evaluate("testFA <- principal(attitude, 2, rotate='varimax')").AsList();
            engine.Evaluate("fa.diagram(testFA)").AsList();
            engine.Evaluate("print(testFA$scores)").AsList();
            // you should always dispose of the REngine properly.
        }
        public static void KMeansR()
        {
            var comInisiasi = @"
                library(class)
                library(cluster)
                #Jika Error loading package, uncomment line 4
                #install.packages(c('clv', 'fpc'))
                library(clv)
                library(fpc)

                ####################################################
                ############## MY KMEANS GONNA START HERE ##########
                ####################################################

                ### INPUT ###
                nItter <- 10
                k <- 3
                x <- USArrests
                y <- iris[1:4]

                ### METHOD ###
                myKmeans <- function(x, k, nItter) {
                  #create random centroid
                  centroid_acak <- sample(c(1:length(x[,1])), k, replace = FALSE)
                  centroid <- x[centroid_acak,1:length(x)]
  
                  #count euclidean distance
                  hitung_jarak <- function(x){
                    sqrt(
                      for(i in 1:length(x)) {
                        euclidean <- ((x[i] - centroid[1:k,i])^2)
                        euclidean <- euclidean + euclidean
                        return(euclidean)
                      }
                    )   
                  }
  
                  #for trace
                  clusterHistory <- vector(nItter, mode='list')
                  centroidHistory <- vector(nItter, mode='list')
                  varianceHistory <- vector(nItter, mode='list')
  
                  for(i in 1:nItter) {
                    jarak <- t(apply(as.matrix(x), 1, hitung_jarak)) #returns matrix of distance
                    clusters <- apply(jarak, 1, which.min)           #assign objects to cluster based the shortest distance
                    centroid <- apply(x, 2, tapply, clusters, mean)  #update centroid
                    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
                    VarQE <- mean(diag(Var))                         #Variance with quantization error
    
                    clusterHistory[[i]] <- clusters
                    centroidHistory[[i]] <- centroid
                    varianceHistory[[i]] <- VarQE
    
                    centroid <- centroid
                  }
  
                  list(clusters=clusterHistory, centroid=centroidHistory, VarQE=varianceHistory)
                }
                ";
            engine.Evaluate(comInisiasi);
            engine.Evaluate("testKmeans <- myKmeans(y, 3, 10)");
            Console.Write(printFromR("testKmeans$centroid"));
            Console.Write(printFromR("testKmeans$clusters"));
            Console.Write(printFromR("testKmeans$VarQE"));

            var comOutput = @"
                clsInteger <- as.integer(testKmeans$clusters[[10]])
                Validcls <- cls.scatt.data(y,clsInteger)
                Intracls <- mean(Validcls$intracls.average) #Output show up in groupbox Validity
                Intercls <- Validcls$intercls.average       #Output shop up in groupbox Validity
                testKmeans$VarQE[[10]]                      #Output shop up in groupbox Validity
                Cluster <- matrix(unlist(testKmeans$clusters[[10]]))
                cbind.data.frame(y,Cluster)                 #Output show up in cluster result dataview
            ";
            engine.Evaluate(comOutput);

            engine.Evaluate("plot(unlist(testKmeans$VarQE),type='n', main='Convergence', xlab='Iteration', ylab='Quantization Error')");
            engine.Evaluate("lines(unlist(testKmeans$VarQE),type='o') #Output show up in groupbox convergence");

            engine.Evaluate("plot.new()");
            engine.Evaluate("clusplot(y,testKmeans$clusters[[10]],main='Cluster Plot',color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot");
        }
        public static void PSOtoR()
        {
            var comInisiasi = @"
                library(class)
                library(cluster)
                library(clv)
                library(fpc)

                ####################################################
                ############## MY PSO GONNA START HERE #############
                ####################################################

                ### INPUT ###
                #Constants PSO
                c1 <- 1.49
                c2 <- 1.49
                w <- 0.72        
                k <- 3
                x <- USArrests 
                y <- iris[1:4]

                ### METHOD ###
                myPSO <- function(x, k, nItter, c1, c2, w) {
                  #Create Random centroid
                  centroid_acak <- sample(c(1:length(x[,1])), k, replace = FALSE)
                  centroid <- x[centroid_acak,1:length(x)]
  
                  #count euclidean distance
                  hitung_jarak <- function(x){
                    sqrt(
                      for(i in 1:length(x)) {
                        euclidean <- ((x[i] - centroid[1:k,i])^2)
                        euclidean <- euclidean + euclidean
                        return(euclidean)
                      }
                    )   
                  }
  
  
                  #Fitness Function Cluster
                  OF <- function(x){
                    jarak <- t(apply(as.matrix(x), 1, hitung_jarak))  #returns matrix of distance
                    clusters <- apply(jarak, 1, which.min)         #assign objects to cluster based the shortest distance
                    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
                    VarQE <- mean(diag(Var))                         #Variance with quantization error
                    tabclust <- diag(1/table(factor(clusters, levels=1:k)))         #Fitness Function
                    quantError <- as.matrix(rowSums(jarak %*% tabclust)/k)          #Fitness Function
                    return(quantError)
                  }
  
                  #Matrix Random Uniform
                  mRU <- function(x)
                    # returns a matrix of size mxn of uniform random variates
                  {
                    return(array(runif(k*length(x)), dim = c(k,length(x))))
                  }

                  #initialize position and velocity
                  mP <- x
                  mV <- matrix(0,k,length(x))
  
                  #count fitness function
                  vF <- OF(x)
  
                  #Matrix Best Solutions
                  mPbest <- mP                 # matrix of `personally best' solutions
                  vFbest <- vF                 # vector of OF of best solutions
                  sGbest <- min(vFbest)            # scalar: best OF-value
                  sgbest <- which.min(vFbest)[1]    # scalar: best solution (counter)
  
                  #for trace
                  clusterHistory <- vector(nItter, mode='list')
                  centroidHistory <- vector(nItter, mode='list')
                  varianceHistory <- vector(nItter, mode='list')
  
                  for(i in 1:nItter) {
                    #update centroid
                    mDV <- c1 * mRU(x) * (mPbest[row.names(centroid),] - centroid) + c2 * mRU(x) * (matrix(mPbest[sgbest,]) - centroid)
                    mV  <- w * mV + mDV
                    centroid  <- centroid + mV
                    centroid <- centroid
    
                    #count fitness function
                    #vF <- OF(x)
                    jarak <- t(apply(as.matrix(x), 1, hitung_jarak))  #returns matrix of distance
                    clusters <- apply(jarak, 1, which.min)         #assign objects to cluster based the shortest distance
                    Var <- apply(jarak, 2, tapply, clusters, mean)   #Variance with quantization error
                    VarQE <- mean(diag(Var))                         #Variance with quantization error
                    tabclust <- diag(1/table(factor(clusters, levels=1:k)))         #Fitness Function
                    quantError <- as.matrix(rowSums(jarak %*% tabclust)/k)          #Fitness Function
                    vF <- quantError
    
                    # find improvements
                    logik <- vF < vFbest        # improved solutions
                    mPbest[logik,] <- mP[logik,]
                    vFbest[logik] <- vF[logik]
    
                    # find best solution
                    if (min(vF) < sGbest){
                      sGbest <- min(vF)
                      sgbest <- which.min(vF)[1]
                    }
    
                    clusterHistory[[i]] <- clusters
                    centroidHistory[[i]] <- centroid
                    varianceHistory[[i]] <- VarQE
                  }
  
                  list(clusters=clusterHistory, centroid=centroidHistory, VarQE=varianceHistory)
                }";
            engine.Evaluate(comInisiasi);

            engine.Evaluate("testPSO <- myPSO(y,k,101,c1,c2,w)");
            Console.Write(printFromR("testPSO$centroid"));
            Console.Write(printFromR("testPSO$clusters"));
            Console.Write(printFromR("testPSO$VarQE"));

            var comOutput = @"
                clsInteger <- as.integer(testPSO$clusters[[100]])
                Validcls <- cls.scatt.data(y,clsInteger)
                Intracls <- mean(Validcls$intracls.average) #Output show up in groupbox Validity
                Intercls <- Validcls$intercls.average       #Output shop up in groupbox Validity
                testPSO$VarQE[[100]]                      #Output shop up in groupbox Validity
                Cluster <- matrix(unlist(testPSO$clusters[[100]]))
                cbind.data.frame(y,Cluster)                 #Output show up in cluster result dataview
            ";
            engine.Evaluate(comOutput);

            engine.Evaluate("plot(unlist(testPSO$VarQE),type='n', main='Convergence', xlab='Iteration', ylab='Quantization Error')");
            engine.Evaluate("lines(unlist(testPSO$VarQE),type='o') #Output show up in groupbox convergence");

            engine.Evaluate("plot.new()");
            engine.Evaluate("clusplot(y,testPSO$clusters[[100]],main='Cluster Plot',color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot");
        }

        public static string printFromR(String command)
        {
            var com = @"
                toPrint <- capture.output({0})
                ";
            engine.Evaluate(string.Format(com, command));
            var m = engine.GetSymbol("toPrint").AsCharacter().ToArray();
            string printOut = "";
            for (int i = 0; i < m.Length; i++)
            {
                printOut += (m[i] + "\n");
            }
            return printOut;
        }

    }
}
