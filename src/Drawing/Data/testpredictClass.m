clear
clc


%run('vlfeat-0.9.20/toolbox/vl_setup');

name = 'C:\Users\emre\Documents\Visual Studio 2015\Projects\IUI Term Project\Drawing\bin\Debug\Data\Test.jpg';
%{
I = imread(name);
I = im2single(I);
I = imresize(I, [1440 2560]);
hog = vl_hog(I, 8, 'verbose');
%}

[predicted, probs] = predictClass(name);

