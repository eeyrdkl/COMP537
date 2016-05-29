function [predicted, probs] = predictClass(picturename)
    load('model.mat');
    run('vlfeat-0.9.20/toolbox/vl_setup');     
    img = imread(picturename);
    img = im2single(img);
    img = imresize(img, [1440 2560]);
    hog = vl_hog(img, 8, 'verbose');
    feature = hog(:);
    [predicted, ~, probs] = svmpredict(zeros(1,1), double(feature'), model, '-b 1');
end

