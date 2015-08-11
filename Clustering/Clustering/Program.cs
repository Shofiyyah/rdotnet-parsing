using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RDotNet;
using System.Text;
using System.Drawing;

namespace Clustering
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static REngine engine;
        public static Form1 form1;

        [STAThread]
        static void Main()
        {
            InitiateR();
            prepR();
            setListener();

            Application.Run(form1);
        }

        private static void InitiateR()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 = new Form1();
            string rhome = System.Environment.GetEnvironmentVariable("R_HOME");
            if (string.IsNullOrEmpty(rhome))
                rhome = @"C:\Program Files\R\R-3.1.1";

            System.Environment.SetEnvironmentVariable("R_HOME", rhome);
            System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + rhome + @"\bin\i386");

            REngine.SetEnvironmentVariables();
            // There are several options to initialize the engine, but by default the following suffice:
            engine = REngine.GetInstance();
            engine.Initialize();
        }
        private static void prepR()
        {
            var prep = @"
                library(psych)
                
                library(class)
                library(cluster)
                library(clv)
                library(fpc)
                
                #new Library by TGS
                #Jika Error loading package, uncomment line dibawah
                #install.packages(c('clv', 'xlsx', 'XLConnect'))
                library(Cairo) 
                library(xlsx)
                library(XLConnect) 
            ";
            var kmeansPrep = @"
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
            var psoPrep = @"
                ####################################################
                ############## MY PSO GONNA START HERE #############
                ####################################################
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
                }
            ";
            engine.Evaluate(prep);
            engine.Evaluate(kmeansPrep);
            engine.Evaluate(psoPrep);
        }
        protected static void disposeR(object sender, EventArgs e)
        {
            engine.Dispose();
        }

        protected static void actionFA(object sender, EventArgs e)
        {
            var KMO = form1.getForm4().getCombo().SelectedIndex;
            var f = form1.getForm4().getNumberFactor();
            var rotate = form1.getForm4().getRotate() ? ", rotate='varimax'" : "";
            var z = form1.getDataPath();

            var comInisiasi = @"
                ### INPUT ###
                z <- read.xlsx('{0}',1)
                z <- z[2:ncol(z)]
                f <- {1}
                ";
            engine.Evaluate(string.Format(comInisiasi, z, f));
            drawChart(form1.getNumberF(), "fa.parallel(attitude)");

            if (KMO == 0) form1.getFAMSA().Text = printFromR("KMO(cor(attitude))");
            else form1.getFAMSA().Text = printFromR("KMO(cov(attitude))");

            engine.Evaluate(string.Format("testFA <- principal(attitude, 2 {0})", rotate));
            drawChart(form1.getFaDiagram(), "fa.diagram(testFA)");

            engine.Evaluate("scoring <- testFA$scores");
            engine.Evaluate("write.xlsx(scoring, 'c:/temp/mydataFA.xls')");
            engine.Evaluate("comunality <- t(testFA$communality)");
            engine.Evaluate("write.xlsx(comunality, 'c:/temp/comunFA.xls')");

            form1.fill("c:/temp/comunFA.xls", form1.getComunalityGrid());
            form1.fill("c:/temp/mydataFA.xls", form1.getScoreGrid());
            form1.setDataPath2("c:/temp/mydataFA.xls");

            form1.getForm4().Close();
            form1.setShow(2);
            form1.setShow1(1);
        }
        protected static void actionKMeansR(object sender, EventArgs e)
        {
            var itter = form1.getForm2().getItter();
            var k = form1.getForm2().getCentro();
            var y = form1.getDataPath();

            var comInisiasi = @"
                ### INPUT ###
                nItter <- {0}
                k <- {1}
                y <- read.xlsx('{2}',1)
                y <- y[2:ncol(y)]
                ";
            engine.Evaluate(string.Format(comInisiasi, itter, k, y));
            engine.Evaluate("testKmeans <- myKmeans(y, k, nItter)");

            var comOutput = @"
                clsInteger <- as.integer(testKmeans$clusters[[nItter]])
                Validcls <- cls.scatt.data(y,clsInteger)
                Intracls <- mean(Validcls$intracls.average)                                     #Intraclass
                preInter <- Validcls$intercls.average                                           ###NEW
                to.upper <- function(X) X[upper.tri(X,diag=FALSE)]                              ###NEW
                Intercls <- mean(to.upper(preInter))                                            ###EDITED
                QuanzError <- testKmeans$VarQE[[nItter]]                                        #Quantization Error
                Cluster <- matrix(unlist(testKmeans$clusters[[nItter]]))
                clusterResultDataView <- cbind.data.frame(as.matrix(row.names(y)),Cluster)      #cluster result dataview
            ";
            engine.Evaluate(comOutput);
            String intraclas = engine.GetSymbol("Intracls").AsNumeric().First().ToString(); form1.getKIntracls().Text = intraclas;
            String interclas = engine.GetSymbol("Intercls").AsNumeric().First().ToString(); form1.getKIntercls().Text = interclas;
            String quanzerr = engine.GetSymbol("QuanzError").AsNumeric().First().ToString(); form1.getKQuanzErr().Text = quanzerr;

            var plotConv = @"
                        plot(unlist(testKmeans$VarQE),type='n', main='Convergence', xlab='Iteration', ylab='Quantization Error')
                        lines(unlist(testKmeans$VarQE),type='o') #Output show up in groupbox convergence
                        ";
            var clustPlot = @"
                            plot.new()
                            clusplot(y,testKmeans$clusters[[nItter]],main='Cluster Plot',color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot
                ";
            drawChart(form1.getKConvergence(), plotConv);
            drawChart(form1.getKClusterPlot(), clustPlot);

            engine.Evaluate("write.xlsx(clusterResultDataView[2:ncol(clusterResultDataView)], 'c:/temp/mydataKCluster.xls')");
            form1.fill("c:/temp/mydataKCluster.xls", form1.getKClusterGrid());

            form1.setShow1(2);
            form1.getForm2().Close();
        }
        protected static void actionPSOtoR(object sender, EventArgs e)
        {
            var c1 = form1.getForm3().getC1();
            var c2 = form1.getForm3().getC2();
            var w = form1.getForm3().getWeight();
            var k = form1.getForm3().getCentro();
            var itter = form1.getForm3().getItter();
            var y = form1.getDataPath();

            var comInisiasi = @"
                ### INPUT ###
                #Constants PSO
                c1 <- {0}
                c2 <- {1}
                w <- {2}        
                k <- {3}
                y <- read.xlsx('{4}',1)
                nItter <- {5}
                y <- y[2:ncol(y)]
                ";

            engine.Evaluate(string.Format(comInisiasi, c1, c2, w, k, y, itter));
            engine.Evaluate("testPSO <- myPSO(y,k,nItter,c1,c2,w)");

            var comOutput = @"
                clsInteger <- as.integer(testPSO$clusters[[nItter]])
                Validcls <- cls.scatt.data(y,clsInteger)
                Intracls <- mean(Validcls$intracls.average) #Output show up in groupbox Validity
                preInter <- Validcls$intercls.average ###NEW
                to.upper <- function(X) X[upper.tri(X,diag=FALSE)] ###NEW
                Intercls <- mean(to.upper(preInter))       ### EDITED:Output shop up in groupbox Validity
                QuanzError <- testPSO$VarQE[[nItter]]                      #Output shop up in groupbox Validity
                Cluster <- matrix(unlist(testPSO$clusters[[nItter]]))
                clusterResultDataView <- cbind.data.frame(y,Cluster)                 #Output show up in cluster result dataview
                ";
            engine.Evaluate(comOutput);
            String intraclas = engine.GetSymbol("Intracls").AsNumeric().First().ToString(); form1.getPIntracls().Text = intraclas;
            String interclas = engine.GetSymbol("Intercls").AsNumeric().First().ToString(); form1.getPIntercls().Text = interclas;
            String quanzerr = engine.GetSymbol("QuanzError").AsNumeric().First().ToString(); form1.getPQuanzErr().Text = quanzerr;

            var plotConv = @"
                        plot(unlist(testPSO$VarQE),type='n', main='Convergence', xlab='Iteration', ylab='Quantization Error')
                        lines(unlist(testPSO$VarQE),type='o') #Output show up in groupbox convergence
                        ";
            var clustPlot = @"
                            plot.new()
                            clusplot(y,testPSO$clusters[[nItter]],main='Cluster Plot',color=TRUE,shade=TRUE, labels=2, lines=0) #Output show up in groupbox cluster plot
                ";
            drawChart(form1.getPConvergence(), plotConv);
            drawChart(form1.getPClusterPlot(), clustPlot);

            engine.Evaluate("write.xlsx(clusterResultDataView[2:ncol(clusterResultDataView)], 'c:/temp/mydataPCluster.xls')");
            form1.fill("c:/temp/mydataPCluster.xls", form1.getPClusterGrid());

            form1.setShow1(3);
            form1.getForm3().Close();
        }

        private static string printFromR(String command)
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
        private static void drawChart(PictureBox pictureBox1, String command)
        {
            StringBuilder plotCommmand = new StringBuilder();

            plotCommmand.Append(@"CairoPNG('c:\\temp\\r.png',bg='transparent');");
            plotCommmand.Append(command + ";\n");
            plotCommmand.Append("graphics.off();");
            engine.Evaluate(plotCommmand.ToString());

            using (System.IO.StreamReader str = new System.IO.StreamReader("c:\\temp\\r.png"))
            {
                pictureBox1.Image = new Bitmap(str.BaseStream);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                str.Close();
            }
            pictureBox1.Invalidate();
        }

        private static void setListener()
        {
            form1.FormClosed += new FormClosedEventHandler(disposeR);
            form1.getForm2().getApply().Click += new EventHandler(actionKMeansR);
            form1.getForm3().getApply().Click += new EventHandler(actionPSOtoR);
            form1.getForm4().getApply().Click += new EventHandler(actionFA);
        }
    }
}
